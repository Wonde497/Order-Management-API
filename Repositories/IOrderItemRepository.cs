using OrderManagementAPI.Models;

namespace OrderManagementAPI.Repositories;

public interface IOrderItemRepository
{
    Task<List<OrderItem>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
    Task<OrderItem?> GetByIdAsync(int orderId, int itemId, CancellationToken cancellationToken = default);
    Task AddAsync(OrderItem item, CancellationToken cancellationToken = default);
    void Remove(OrderItem item);
}