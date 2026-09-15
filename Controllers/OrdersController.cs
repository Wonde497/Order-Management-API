using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.DTOs;
using OrderManagementAPI.Models;
using OrderManagementAPI.Services;

namespace OrderManagementAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<OrderResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<OrderResponseDto>>> GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? customerId = null,
        [FromQuery] OrderStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _orderService.GetPagedAsync(page, pageSize, customerId, status, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponseDto>> GetOrderById(int id, CancellationToken cancellationToken = default)
    {
        var order = await _orderService.GetByIdAsync(id, cancellationToken);
        if (order is null)
            return NotFound($"Order with ID {id} not found.");

        return Ok(order);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder(CreateOrderDto dto, CancellationToken cancellationToken = default)
    {
        var (order, error) = await _orderService.CreateAsync(dto, cancellationToken);
        if (error is not null)
            return Conflict(error);

        return CreatedAtAction(nameof(GetOrderById), new { id = order!.Id }, order);
    }

    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderResponseDto>> UpdateOrderStatus(
        int id, 
        [FromBody] UpdateOrderStatusDto dto, 
        CancellationToken cancellationToken = default)
    {
        var (order, error) = await _orderService.UpdateStatusAsync(id, dto, cancellationToken);

        if (error == "NOT_FOUND")
            return NotFound($"Order with ID {id} not found.");
        if (error is not null)
            return Conflict(error);

        return Ok(order);
    }
}