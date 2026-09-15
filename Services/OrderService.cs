using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.DTOs;
using OrderManagementAPI.Models;
using OrderManagementAPI.Repositories;

namespace OrderManagementAPI.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
    }

    public async Task<PagedResult<OrderResponseDto>> GetPagedAsync(
        int page, 
        int pageSize, 
        int? customerId, 
        OrderStatus? status, 
        CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 10 : pageSize;

        var (items, totalCount) = await _orderRepository.GetPagedAsync(page, pageSize, customerId, status, cancellationToken);
        var dtos = items.Select(MapToResponseDto).ToList();

        return new PagedResult<OrderResponseDto>(dtos, page, pageSize, totalCount);
    }

    public async Task<OrderResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        return order is null ? null : MapToResponseDto(order);
    }

    public async Task<(OrderResponseDto? Order, string? Error)> CreateAsync(
        CreateOrderDto dto, 
        CancellationToken cancellationToken = default)
    {
        var customerExists = await _customerRepository.GetByIdAsync(dto.CustomerId, cancellationToken);
        if (customerExists is null)
            return (null, $"Customer with ID {dto.CustomerId} does not exist.");

        var order = new Order
        {
            CustomerId = dto.CustomerId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        decimal totalAmount = 0;

        foreach (var itemDto in dto.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId, cancellationToken);
            if (product is null)
                return (null, $"Product with ID {itemDto.ProductId} does not exist.");

            if (product.StockQuantity < itemDto.Quantity)
                return (null, $"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {itemDto.Quantity}.");

            // Decrement product inventory
            product.StockQuantity -= itemDto.Quantity;

            var itemTotal = product.Price * itemDto.Quantity;
            totalAmount += itemTotal;

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price
            });
        }

        order.TotalAmount = totalAmount;

        try
        {
            await _orderRepository.AddAsync(order, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return (null, "A concurrency conflict occurred while processing stock quantities. Please try placing your order again.");
        }

        return (MapToResponseDto(order), null);
    }

    public async Task<(OrderResponseDto? Order, string? Error)> UpdateStatusAsync(
        int id, 
        UpdateOrderStatusDto dto, 
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order is null)
            return (null, "NOT_FOUND");

        _orderRepository.SetOriginalRowVersion(order, dto.RowVersion);

        order.Status = dto.Status;
        order.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _orderRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return (null, "The order was updated by another user. Please refresh and try again.");
        }

        return (MapToResponseDto(order), null);
    }

    private static OrderResponseDto MapToResponseDto(Order order) =>
        new(
            order.Id,
            order.CustomerId,
            order.Status,
            order.TotalAmount,
            order.CreatedAt,
            order.UpdatedAt,
            order.RowVersion,
            order.Items.Select(i => new OrderItemResponseDto(
                i.Id,
                i.ProductId,
                i.Product?.Name ?? string.Empty,
                i.Quantity,
                i.UnitPrice,
                i.Quantity * i.UnitPrice
            )).ToList()
        );
}