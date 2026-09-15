using OrderManagementAPI.DTOs;

namespace OrderManagementAPI.Services;

public interface IAuthService
{
    Task<(AuthResponseDto? Result, string? Error)> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken);
    Task<(AuthResponseDto? Result, string? Error)> LoginAsync(LoginDto dto, CancellationToken cancellationToken);
}
