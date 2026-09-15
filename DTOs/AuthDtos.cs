using System.ComponentModel.DataAnnotations;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.DTOs;

public record RegisterDto(
    [Required, StringLength(50, MinimumLength = 3)] string Username,
    [Required, StringLength(100, MinimumLength = 8)] string Password,
    UserRole Role
);

public record LoginDto(
    [Required] string Username,
    [Required] string Password
);

// What the client gets back after a successful login/register.
public record AuthResponseDto(
    string Token,
    string Username,
    UserRole Role,
    DateTime ExpiresAtUtc
);
