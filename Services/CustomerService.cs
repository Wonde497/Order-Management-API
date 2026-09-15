using OrderManagementAPI.DTOs;
using OrderManagementAPI.Models;
using OrderManagementAPI.Repositories;

namespace OrderManagementAPI.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    private static CustomerResponseDto ToDto(Customer c) =>
        new(c.Id, c.FullName, c.Email, c.Phone, c.CreatedAt);

    public async Task<CustomerResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(id, cancellationToken);
        return customer is null ? null : ToDto(customer);
    }

    public async Task<PagedResult<CustomerResponseDto>> GetPagedAsync(
        int page, int pageSize, string? search, CancellationToken cancellationToken)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var (items, totalCount) = await _repository.GetPagedAsync(page, pageSize, search, cancellationToken);
        var dtoItems = items.Select(ToDto).ToList();

        return new PagedResult<CustomerResponseDto>(dtoItems, page, pageSize, totalCount);
    }

    public async Task<(CustomerResponseDto? Customer, string? Error)> CreateAsync(
        CreateCustomerDto dto, CancellationToken cancellationToken)
    {
        var emailExists = await _repository.EmailExistsAsync(dto.Email, null, cancellationToken);
        if (emailExists)
            return (null, $"A customer with email '{dto.Email}' already exists.");

        var customer = new Customer
        {
            FullName = dto.Fullname,
            Email = dto.Email,
            Phone = dto.Phone
        };

        await _repository.AddAsync(customer, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return (ToDto(customer), null);
    }

    public async Task<(CustomerResponseDto? Customer, string? Error)> UpdateAsync(
        int id, UpdateCustomerDto dto, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
            return (null, "NOT_FOUND");

        var emailTaken = await _repository.EmailExistsAsync(dto.Email, id, cancellationToken);
        if (emailTaken)
            return (null, $"Email '{dto.Email}' is already used by another customer.");

        customer.FullName = dto.Fullname;
        customer.Email = dto.Email;
        customer.Phone = dto.Phone;

        await _repository.SaveChangesAsync(cancellationToken);

        return (ToDto(customer), null);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(id, cancellationToken);
        if (customer is null) return false;

        await _repository.DeleteAsync(customer, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task DeleteAllAsync(CancellationToken cancellationToken)
    {
        await _repository.DeleteAllAsync(cancellationToken);
    }
}
