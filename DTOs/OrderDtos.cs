using System.ComponentModel.DataAnnotations;
using OrderManagementAPI.Models;

namespace OrderManagementAPI.DTOs;

public record OrderItemResponseDto(
    int Id,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);

public record OrderResponseDto(
    int Id,
    int CustomerId,
    OrderStatus Status,
    decimal TotalAmount,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    uint RowVersion,
    List<OrderItemResponseDto> Items
);

public record CreateOrderItemDto(
    [Required] int ProductId,
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")] int Quantity
);

public record CreateOrderDto(
    [Required] int CustomerId,
    [Required, MinLength(1, ErrorMessage = "An order must contain at least one item.")] List<CreateOrderItemDto> Items
);

public record UpdateOrderStatusDto(
    [Required] OrderStatus Status,
    [Required] uint RowVersion
);