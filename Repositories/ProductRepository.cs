using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Data; // Adjust to your DbContext namespace
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly OrderManagementAPIContext _context;

    public ProductRepository(OrderManagementAPIContext context)
    {
        _context = context;
    }

    public async Task<(List<Product> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? search, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchPattern = $"%{search.Trim()}%";
            query = query.Where(p => EF.Functions.Like(p.Name, searchPattern) || EF.Functions.Like(p.Sku, searchPattern));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<bool> ExistsBySkuAsync(string sku, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsNoTracking().Where(p => p.Sku == sku);

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }

    public void Remove(Product product)
    {
        _context.Products.Remove(product);
    }

    public async Task DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        // ExecuteDeleteAsync performs a direct SQL bulk delete for high performance
        await _context.Products.ExecuteDeleteAsync(cancellationToken);
    }

    public void SetOriginalRowVersion(Product product, uint rowVersion)
    {
        _context.Entry(product).Property(p => p.RowVersion).OriginalValue = rowVersion;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}