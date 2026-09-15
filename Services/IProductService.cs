using OrderManagementAPI.DTOs;

namespace OrderManagementAPI.Services;

public interface IProductService
{
    Task<PagedResult<ProductResponseDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<ProductResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(ProductResponseDto? Product, string? Error)> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
    Task<(ProductResponseDto? Product, string? Error)> UpdateAsync(int id, UpdateProductDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteAllAsync(CancellationToken cancellationToken = default);
}