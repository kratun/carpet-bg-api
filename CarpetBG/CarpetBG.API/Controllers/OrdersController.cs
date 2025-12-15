using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.Interfaces.Services;

using Microsoft.AspNetCore.Mvc;

namespace CarpetBG.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderService orderService, IOrderItemService orderItemService) : ControllerBase
{
    [HttpGet]
    //[Authorize]
    public async Task<IActionResult> GetAll([FromQuery] OrderFilterDto dto)
    {
        var result = await orderService.GetFilteredAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpGet]
    [Route("{id}")]
    //[Authorize]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await orderService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPost]
    //[Authorize]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        var result = await orderService.CreateOrderAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPost]
    [Route("{id}/order-items")]
    //[Authorize]
    public async Task<IActionResult> AddOrderItem([FromRoute] Guid id, [FromBody] OrderItemDto dto)
    {
        var result = await orderItemService.AddAsync(dto, id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPut]
    [Route("{id}/status")]
    //[Authorize]
    public async Task<IActionResult> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateOrderStatusDto dto)
    {
        var result = await orderService.UpdateOrderStatusAsync(id, dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPut]
    [Route("{id}/status-revert")]
    //[Authorize]
    public async Task<IActionResult> RevertStatus([FromRoute] Guid id, [FromBody] UpdateOrderStatusDto dto)
    {
        var result = await orderService.RevertOrderStatusAsync(id, dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPut]
    [Route("{id}/delivery-data")]
    //[Authorize]
    public async Task<IActionResult> AddDeliveryData([FromRoute] Guid id, [FromBody] OrderDeliveryDataDto dto)
    {
        var result = await orderService.AddDeliveryDataAsync(id, dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPut]
    [Route("{id}/delivery-confirm")]
    //[Authorize]
    public async Task<IActionResult> ConfirmDelivery([FromRoute] Guid id, [FromBody] OrderDeliveryConfirmDto dto)
    {
        var result = await orderService.ConfirmDeliveryAsync(id, dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }
}
