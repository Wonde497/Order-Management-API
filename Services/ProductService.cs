using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.DTOs;
using OrderManagementAPI.Models;
using OrderManagementAPI.Repositories;
 // Adjust to your Repository namespace

namespace OrderManagementAPI.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ProductResponseDto>> GetPagedAsync(
        int page, 
        int pageSize, 
        string? search, 
        CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 10 : pageSize;

        var (items, totalCount) = await _productRepository.GetPagedAsync(page, pageSize, search, cancellationToken);

        var dtos = items.Select(MapToResponseDto).ToList();

        return new PagedResult<ProductResponseDto>(dtos, page, pageSize, totalCount);
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : MapToResponseDto(product);
    }

    public async Task<(ProductResponseDto? Product, string? Error)> CreateAsync(
        CreateProductDto dto, 
        CancellationToken cancellationToken = default)
    {
        if (await _productRepository.ExistsBySkuAsync(dto.Sku, null, cancellationToken))
            return (null, $"A product with SKU '{dto.Sku}' already exists.");

        var product = new Product
        {
            Name = dto.Name,
            Sku = dto.Sku,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            CreatedAt = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return (MapToResponseDto(product), null);
    }

    public async Task<(ProductResponseDto? Product, string? Error)> UpdateAsync(
        int id, 
        UpdateProductDto dto, 
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
            return (null, "NOT_FOUND");

        if (await _productRepository.ExistsBySkuAsync(dto.Sku, id, cancellationToken))
            return (null, $"A product with SKU '{dto.Sku}' already exists.");

        // Set original RowVersion to handle optimistic concurrency
        _productRepository.SetOriginalRowVersion(product, dto.RowVersion);

        product.Name = dto.Name;
        product.Sku = dto.Sku;
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;

        try
        {
            await _productRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return (null, "The record was modified by another user. Please refresh and try again.");
        }

        return (MapToResponseDto(product), null);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
            return false;

        _productRepository.Remove(product);
        await _productRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        await _productRepository.DeleteAllAsync(cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);
    }

    private static ProductResponseDto MapToResponseDto(Product product) =>
        new(
            product.Id,
            product.Name,
            product.Sku,
            product.Price,
            product.StockQuantity,
            product.CreatedAt,
            product.RowVersion
        );
}