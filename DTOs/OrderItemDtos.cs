using System.ComponentModel.DataAnnotations;

namespace OrderManagementAPI.DTOs;

public record AddOrderItemDto(
    [Required] int ProductId,
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")] int Quantity,
    [Required] uint OrderRowVersion
);

public record UpdateOrderItemQuantityDto(
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")] int Quantity,
    [Required] uint OrderRowVersion
);