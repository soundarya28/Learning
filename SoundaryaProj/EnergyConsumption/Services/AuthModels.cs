using System;
using System.ComponentModel.DataAnnotations;

namespace EnergyConsumption.Services;

public sealed record RegisterRequest
{
    [Required]
    public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; init; } = string.Empty;
}

public sealed record LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}

public sealed record AuthResponse
{
    public string Token { get; init; } = string.Empty;
    public DateTime Expires { get; init; }
    public UserDto User { get; init; } = default!;
}

public sealed record UserDto(int UserId, string Name, string Email, bool IsActive, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);
