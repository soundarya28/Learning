using System.Threading;
using System.Threading.Tasks;

namespace EnergyConsumption.Services;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterRequest model, CancellationToken cancellationToken);
    Task<AuthResponse> LoginAsync(LoginRequest model, CancellationToken cancellationToken);
    Task<UserDto?> GetUserAsync(int userId, CancellationToken cancellationToken);
}
