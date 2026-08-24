using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Entities;
using SoundaryaProj.EnergyConsumption.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EnergyConsumption.Services;

public class AuthService(
    ApplicationDbContext db,
    IPasswordHasher<User> passwordHasher,
    IConfiguration config,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    private readonly IConfiguration _config = config ?? throw new ArgumentNullException(nameof(config));
    private readonly ILogger<AuthService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<UserDto> RegisterAsync(RegisterRequest model, CancellationToken cancellationToken)
    {
        var email = model.Email.Trim().ToLowerInvariant();

        if (await _db.Users.AnyAsync(u => u.Email == email, cancellationToken))
            throw new InvalidOperationException("Email is already in use.");

        var user = new User

        {
            Name = model.Name.Trim(),
            Email = email,
            IsActive = true
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        return new UserDto(user.UserId, user.Name, user.Email, user.IsActive, user.CreatedAt, user.UpdatedAt);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest model, CancellationToken cancellationToken)
    {
        var email = model.Email.Trim().ToLowerInvariant();

        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (verify == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = CreateJwtToken(user);

        return new AuthResponse
        {
            Token = token.Token,
            Expires = token.Expires,
            User = new UserDto(user.UserId, user.Name, user.Email, user.IsActive, user.CreatedAt, user.UpdatedAt)
        };
    }

    public async Task<UserDto?> GetUserAsync(int userId, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null) return null;
        return new UserDto(user.UserId, user.Name, user.Email, user.IsActive, user.CreatedAt, user.UpdatedAt);
    }

    private (string Token, DateTime Expires) CreateJwtToken(User user)
    {
        var key = _config["Jwt:Key"];
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];

        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException("JWT signing key is not configured. Set 'Jwt:Key' in configuration.");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenString, expires);
    }
}
