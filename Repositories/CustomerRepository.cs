using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Data;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly OrderManagementAPIContext _context;

    public CustomerRepository(OrderManagementAPIContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Customers.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<(List<Customer> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? search, CancellationToken cancellationToken)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c =>
                EF.Functions.ILike(c.FullName, $"%{search}%") ||
                EF.Functions.ILike(c.Email, $"%{search}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeCustomerId, CancellationToken cancellationToken)
    {
        var query = _context.Customers.Where(c => c.Email == email);

        if (excludeCustomerId.HasValue)
            query = query.Where(c => c.Id != excludeCustomerId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        await _context.Customers.AddAsync(customer, cancellationToken);
    }

    public Task DeleteAsync(Customer customer, CancellationToken cancellationToken)
    {
        _context.Customers.Remove(customer);
        return Task.CompletedTask;
    }

    public async Task DeleteAllAsync(CancellationToken cancellationToken)
    {
        await _context.Customers.ExecuteDeleteAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
