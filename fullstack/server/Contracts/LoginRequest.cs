using System.ComponentModel.DataAnnotations;

namespace fullstackapplication.Server.Contracts;

public sealed class LoginRequest
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Username { get; init; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;
}
