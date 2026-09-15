using OrderManagementAPI.DTOs;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.Services;

public interface IOrderService
{
    Task<PagedResult<OrderResponseDto>> GetPagedAsync(
        int page, 
        int pageSize, 
        int? customerId, 
        OrderStatus? status, 
        CancellationToken cancellationToken = default);

    Task<OrderResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<(OrderResponseDto? Order, string? Error)> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken = default);

    Task<(OrderResponseDto? Order, string? Error)> UpdateStatusAsync(int id, UpdateOrderStatusDto dto, CancellationToken cancellationToken = default);
}