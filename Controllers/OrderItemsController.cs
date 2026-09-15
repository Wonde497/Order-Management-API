using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.DTOs;
using OrderManagementAPI.Services;

namespace OrderManagementAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orders/{orderId:int}/items")]
public class OrderItemsController : ControllerBase
{
    private readonly IOrderItemService _orderItemService;

    public OrderItemsController(IOrderItemService orderItemService)
    {
        _orderItemService = orderItemService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<OrderItemResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<OrderItemResponseDto>>> GetOrderItems(int orderId, CancellationToken cancellationToken = default)
    {
        var items = await _orderItemService.GetItemsByOrderIdAsync(orderId, cancellationToken);
        if (items is null)
            return NotFound($"Order with ID {orderId} not found.");

        return Ok(items);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderItemResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderItemResponseDto>> AddOrderItem(
        int orderId, 
        [FromBody] AddOrderItemDto dto, 
        CancellationToken cancellationToken = default)
    {
        var (item, error) = await _orderItemService.AddItemAsync(orderId, dto, cancellationToken);

        if (error == "ORDER_NOT_FOUND")
            return NotFound($"Order with ID {orderId} not found.");
        if (error == "CONCURRENCY_CONFLICT")
            return Conflict("The order was modified by another operation. Please refresh and try again.");
        if (error is not null)
            return BadRequest(error);

        return CreatedAtAction(nameof(GetOrderItems), new { orderId }, item);
    }

    [HttpPut("{itemId:int}")]
    [ProducesResponseType(typeof(OrderItemResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderItemResponseDto>> UpdateOrderItemQuantity(
        int orderId, 
        int itemId, 
        [FromBody] UpdateOrderItemQuantityDto dto, 
        CancellationToken cancellationToken = default)
    {
        var (item, error) = await _orderItemService.UpdateQuantityAsync(orderId, itemId, dto, cancellationToken);

        if (error == "ORDER_NOT_FOUND")
            return NotFound($"Order with ID {orderId} not found.");
        if (error == "ITEM_NOT_FOUND")
            return NotFound($"Item with ID {itemId} not found in Order {orderId}.");
        if (error == "CONCURRENCY_CONFLICT")
            return Conflict("The order was modified by another operation. Please refresh and try again.");
        if (error is not null)
            return BadRequest(error);

        return Ok(item);
    }

    [HttpDelete("{itemId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RemoveOrderItem(
        int orderId, 
        int itemId, 
        [FromQuery] uint orderRowVersion, 
        CancellationToken cancellationToken = default)
    {
        var (success, error) = await _orderItemService.RemoveItemAsync(orderId, itemId, orderRowVersion, cancellationToken);

        if (error == "ORDER_NOT_FOUND")
            return NotFound($"Order with ID {orderId} not found.");
        if (error == "ITEM_NOT_FOUND")
            return NotFound($"Item with ID {itemId} not found in Order {orderId}.");
        if (error == "CONCURRENCY_CONFLICT")
            return Conflict("The order was modified by another operation. Please refresh and try again.");
        if (error is not null)
            return BadRequest(error);

        return NoContent();
    }
}