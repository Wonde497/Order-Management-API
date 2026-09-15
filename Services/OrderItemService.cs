using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.DTOs;
using OrderManagementAPI.Models;
using OrderManagementAPI.Repositories;

namespace OrderManagementAPI.Services;

public class OrderItemService : IOrderItemService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IProductRepository _productRepository;

    public OrderItemService(
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _productRepository = productRepository;
    }

    public async Task<List<OrderItemResponseDto>?> GetItemsByOrderIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null) return null;

        var items = await _orderItemRepository.GetByOrderIdAsync(orderId, cancellationToken);
        return items.Select(MapToResponseDto).ToList();
    }

    public async Task<(OrderItemResponseDto? Item, string? Error)> AddItemAsync(int orderId, AddOrderItemDto dto, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null) return (null, "ORDER_NOT_FOUND");

        if (order.Status != OrderStatus.Pending)
            return (null, "Only pending orders can be modified.");

        _orderRepository.SetOriginalRowVersion(order, dto.OrderRowVersion);

        var product = await _productRepository.GetByIdAsync(dto.ProductId, cancellationToken);
        if (product is null) return (null, $"Product with ID {dto.ProductId} not found.");

        if (product.StockQuantity < dto.Quantity)
            return (null, $"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}.");

        product.StockQuantity -= dto.Quantity;

        var item = new OrderItem
        {
            OrderId = orderId,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            UnitPrice = product.Price // Snapshot price
        };

        order.TotalAmount += item.UnitPrice * item.Quantity;
        order.UpdatedAt = DateTime.UtcNow;

        await _orderItemRepository.AddAsync(item, cancellationToken);

        try
        {
            await _orderRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return (null, "CONCURRENCY_CONFLICT");
        }

        return (MapToResponseDto(item), null);
    }

    public async Task<(OrderItemResponseDto? Item, string? Error)> UpdateQuantityAsync(int orderId, int itemId, UpdateOrderItemQuantityDto dto, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null) return (null, "ORDER_NOT_FOUND");

        if (order.Status != OrderStatus.Pending)
            return (null, "Only pending orders can be modified.");

        var item = await _orderItemRepository.GetByIdAsync(orderId, itemId, cancellationToken);
        if (item is null) return (null, "ITEM_NOT_FOUND");

        _orderRepository.SetOriginalRowVersion(order, dto.OrderRowVersion);

        int quantityDelta = dto.Quantity - item.Quantity;

        if (quantityDelta > 0)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is null || product.StockQuantity < quantityDelta)
                return (null, "Insufficient stock available to increase quantity.");

            product.StockQuantity -= quantityDelta;
        }
        else if (quantityDelta < 0)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is not null)
            {
                product.StockQuantity += Math.Abs(quantityDelta);
            }
        }

        order.TotalAmount += item.UnitPrice * quantityDelta;
        order.UpdatedAt = DateTime.UtcNow;
        item.Quantity = dto.Quantity;

        try
        {
            await _orderRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return (null, "CONCURRENCY_CONFLICT");
        }

        return (MapToResponseDto(item), null);
    }

    public async Task<(bool Success, string? Error)> RemoveItemAsync(int orderId, int itemId, uint orderRowVersion, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null) return (false, "ORDER_NOT_FOUND");

        if (order.Status != OrderStatus.Pending)
            return (false, "Only pending orders can be modified.");

        var item = await _orderItemRepository.GetByIdAsync(orderId, itemId, cancellationToken);
        if (item is null) return (false, "ITEM_NOT_FOUND");

        _orderRepository.SetOriginalRowVersion(order, orderRowVersion);

        // Restore stock quantity back to the product
        var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
        if (product is not null)
        {
            product.StockQuantity += item.Quantity;
        }

        order.TotalAmount -= item.UnitPrice * item.Quantity;
        order.UpdatedAt = DateTime.UtcNow;

        _orderItemRepository.Remove(item);

        try
        {
            await _orderRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return (false, "CONCURRENCY_CONFLICT");
        }

        return (true, null);
    }

    private static OrderItemResponseDto MapToResponseDto(OrderItem item) =>
        new(
            item.Id,
            item.ProductId,
            item.Product?.Name ?? string.Empty,
            item.Quantity,
            item.UnitPrice,
            item.Quantity * item.UnitPrice
        );
}