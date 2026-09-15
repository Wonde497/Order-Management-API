using System.ComponentModel.DataAnnotations;

namespace OrderManagementAPI.DTOs;

public record ProductResponseDto(
    int Id,
    string Name,
    string Sku,
    decimal Price,
    int StockQuantity,
    DateTime CreatedAt,
    uint RowVersion
);

public record CreateProductDto(
    [Required, MaxLength(200)] string Name,
    [Required, MaxLength(50)] string Sku,
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Price cannot be negative.")] decimal Price,
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")] int StockQuantity
);

public record UpdateProductDto(
    [Required, MaxLength(200)] string Name,
    [Required, MaxLength(50)] string Sku,
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Price cannot be negative.")] decimal Price,
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")] int StockQuantity,
    [Required] uint RowVersion
);