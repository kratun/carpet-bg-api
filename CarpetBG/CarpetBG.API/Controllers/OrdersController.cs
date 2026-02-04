using CarpetBG.Application.DTOs.Orders;
using CarpetBG.Application.Interfaces.Services;
using CarpetBG.Infrastructure.Authorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarpetBG.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderService orderService, IOrderItemService orderItemService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PolicyConstants.OperatorAccess)]
    public async Task<IActionResult> GetAll([FromQuery] OrderFilterDto dto)
    {
        var result = await orderService.GetFilteredAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpGet("setup-logistic-data")]
    [Authorize(Policy = PolicyConstants.OperatorAccess)]
    public async Task<IActionResult> GetSetupLogistic([FromQuery] OrderFilterDto dto)
    {
        var result = await orderService.GetSetupLogisticDataAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpGet]
    [Route("{id}")]
    [Authorize(Policy = PolicyConstants.OperatorAccess)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await orderService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPost]
    [Authorize(Policy = PolicyConstants.OperatorAccess)]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        var result = await orderService.CreateOrderAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPost]
    [Route("{id}/order-items")]
    [Authorize(Policy = PolicyConstants.OperatorAccess)]
    public async Task<IActionResult> AddOrderItem([FromRoute] Guid id, [FromBody] OrderItemDto dto)
    {
        var result = await orderItemService.AddAsync(dto, id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = PolicyConstants.OperatorAccess)]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] CreateOrderDto dto)
    {
        var result = await orderService.CreateOrderAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPut]
    [Route("reorder")]
    [Authorize(Policy = PolicyConstants.OperatorAccess)]
    public async Task<IActionResult> Reorder([FromBody] List<OrderDto> dtos)
    {
        var result = await orderService.SetOrderByAsync(dtos);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPut]
    [Route("{orderId}/order-items/{id}")]
    [Authorize(Policy = PolicyConstants.OperatorAccess)]
    public async Task<IActionResult> UpdateOrderItem([FromRoute] Guid orderId, [FromRoute] Guid id, [FromBody] OrderItemDto dto)
    {
        var result = await orderItemService.UpdateAsync(id, dto, orderId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPut]
    [Route("{orderId}/order-items/{id}/complete-washing")]
    [Authorize(Policy = PolicyConstants.OperatorAccess)]
    public async Task<IActionResult> CompleteWashingOrderItem([FromRoute] Guid orderId, [FromRoute] Guid id)
    {
        var result = await orderItemService.CompleteWashingAsync(id, orderId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPut]
    [Route("{id}/status")]
    [Authorize(Policy = PolicyConstants.OperatorAccess)]
    public async Task<IActionResult> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateOrderStatusDto dto)
    {
        var result = await orderService.UpdateOrderStatusAsync(id, dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPut]
    [Route("{id}/status-revert")]
    [Authorize(Policy = PolicyConstants.OperatorAccess)]
    public async Task<IActionResult> RevertStatus([FromRoute] Guid id, [FromBody] UpdateOrderStatusDto dto)
    {
        var result = await orderService.RevertOrderStatusAsync(id, dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPut]
    [Route("{id}/delivery-data")]
    [Authorize(Policy = PolicyConstants.OperatorAccess)]
    public async Task<IActionResult> AddDeliveryData([FromRoute] Guid id, [FromBody] OrderDeliveryDataDto dto)
    {
        var result = await orderService.AddDeliveryDataAsync(id, dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPut]
    [Route("{id}/delivery-confirm")]
    [Authorize(Policy = PolicyConstants.OperatorAccess)]
    public async Task<IActionResult> ConfirmDelivery([FromRoute] Guid id, [FromBody] OrderDeliveryConfirmDto dto)
    {
        var result = await orderService.ConfirmDeliveryAsync(id, dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPut]
    [Route("{id}/complete-washing")]
    [Authorize(Policy = PolicyConstants.OperatorAccess)]
    public async Task<IActionResult> CompleteWashingAsync([FromRoute] Guid id)
    {
        var result = await orderService.CompleteWashingAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }
}
