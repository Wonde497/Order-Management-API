using OrderManagementAPI.Models;

namespace OrderManagementAPI.Repositories;

public interface IProductRepository
{
    Task<(List<Product> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? search, 
        CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsBySkuAsync(string sku, int? excludeId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    void Remove(Product product);

    Task DeleteAllAsync(CancellationToken cancellationToken = default);

    void SetOriginalRowVersion(Product product, uint rowVersion);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}