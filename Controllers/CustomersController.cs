using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.DTOs;
using OrderManagementAPI.Services;

namespace OrderManagementAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CustomerResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<CustomerResponseDto>>> GetCustomers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _customerService.GetPagedAsync(page, pageSize, search, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponseDto>> GetCustomerById(int id, CancellationToken cancellationToken = default)
    {
        var customer = await _customerService.GetByIdAsync(id, cancellationToken);
        if (customer is null)
            return NotFound($"Customer with ID {id} not found.");

        return Ok(customer);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CustomerResponseDto>> CreateCustomer(CreateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        var (customer, error) = await _customerService.CreateAsync(dto, cancellationToken);
        if (error is not null)
            return Conflict(error);

        return CreatedAtAction(nameof(GetCustomerById), new { id = customer!.Id }, customer);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomerResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CustomerResponseDto>> UpdateCustomer(int id, UpdateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        var (customer, error) = await _customerService.UpdateAsync(id, dto, cancellationToken);

        if (error == "NOT_FOUND")
            return NotFound($"Customer with ID {id} not found.");
        if (error is not null)
            return Conflict(error);

        return Ok(customer);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles="Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCustomer(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _customerService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound($"Customer with ID {id} not found.");

        return NoContent();
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAllCustomers(CancellationToken cancellationToken = default)
    {
        await _customerService.DeleteAllAsync(cancellationToken);
        return NoContent();
    }
}
