using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Data;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly OrderManagementAPIContext _context;

    public OrderItemRepository(OrderManagementAPIContext context)
    {
        _context = context;
    }

    public async Task<List<OrderItem>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return await _context.OrderItems
            .AsNoTracking()
            .Include(i => i.Product)
            .Where(i => i.OrderId == orderId)
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderItem?> GetByIdAsync(int orderId, int itemId, CancellationToken cancellationToken = default)
    {
        return await _context.OrderItems
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.OrderId == orderId && i.Id == itemId, cancellationToken);
    }

    public async Task AddAsync(OrderItem item, CancellationToken cancellationToken = default)
    {
        await _context.OrderItems.AddAsync(item, cancellationToken);
    }

    public void Remove(OrderItem item)
    {
        _context.OrderItems.Remove(item);
    }
}