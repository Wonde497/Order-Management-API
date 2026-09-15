using OrderManagementAPI.DTOs;
using OrderManagementAPI.Models;
using OrderManagementAPI.Repositories;

namespace OrderManagementAPI.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task<(AuthResponseDto? Result, string? Error)> RegisterAsync(
        RegisterDto dto, CancellationToken cancellationToken)
    {
        var usernameExists = await _userRepository.UsernameExistsAsync(dto.Username, cancellationToken);
        if (usernameExists)
            return (null, $"Username '{dto.Username}' is already taken.");

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = _passwordService.HashPassword(dto.Password),
            Role = dto.Role
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        var (token, expiresAt) = _jwtService.GenerateToken(user);
        return (new AuthResponseDto(token, user.Username, user.Role, expiresAt), null);
    }

    public async Task<(AuthResponseDto? Result, string? Error)> LoginAsync(
        LoginDto dto, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username, cancellationToken);

        // Deliberately vague - never reveal whether username or password was the wrong part.
        if (user is null || !_passwordService.VerifyPassword(user.PasswordHash, dto.Password))
            return (null, "Invalid username or password.");

        var (token, expiresAt) = _jwtService.GenerateToken(user);
        return (new AuthResponseDto(token, user.Username, user.Role, expiresAt), null);
    }
}
