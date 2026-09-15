namespace OrderManagementAPI.DTOs;
public record CustomerResponseDto(
    int Id,
    string FullName,
    string Email,
    string? Phone,
    DateTime CreatedAt
);