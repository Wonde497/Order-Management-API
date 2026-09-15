using OrderManagementAPI.DTOs;

namespace OrderManagementAPI.Services;

public interface ICustomerService
{
    Task<CustomerResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<PagedResult<CustomerResponseDto>> GetPagedAsync(
        int page, int pageSize, string? search, CancellationToken cancellationToken);

    // Returns null on success with the created customer, or a string error message on failure.
    // (An alternative to exceptions for expected business failures like "email taken".)
    Task<(CustomerResponseDto? Customer, string? Error)> CreateAsync(
        CreateCustomerDto dto, CancellationToken cancellationToken);

    Task<(CustomerResponseDto? Customer, string? Error)> UpdateAsync(
        int id, UpdateCustomerDto dto, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task DeleteAllAsync(CancellationToken cancellationToken);
}
