using OrderManagementAPI.Models;

namespace OrderManagementAPI.Repositories;

public interface IOrderRepository
{
    Task<(List<Order> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        int? customerId, 
        OrderStatus? status, 
        CancellationToken cancellationToken = default);

    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    void SetOriginalRowVersion(Order order, uint rowVersion);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}