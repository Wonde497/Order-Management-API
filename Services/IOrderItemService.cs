using OrderManagementAPI.DTOs;

namespace OrderManagementAPI.Services;

public interface IOrderItemService
{
    Task<List<OrderItemResponseDto>?> GetItemsByOrderIdAsync(int orderId, CancellationToken cancellationToken = default);
    Task<(OrderItemResponseDto? Item, string? Error)> AddItemAsync(int orderId, AddOrderItemDto dto, CancellationToken cancellationToken = default);
    Task<(OrderItemResponseDto? Item, string? Error)> UpdateQuantityAsync(int orderId, int itemId, UpdateOrderItemQuantityDto dto, CancellationToken cancellationToken = default);
    Task<(bool Success, string? Error)> RemoveItemAsync(int orderId, int itemId, uint orderRowVersion, CancellationToken cancellationToken = default);
}