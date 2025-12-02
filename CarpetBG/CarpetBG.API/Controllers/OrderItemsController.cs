using CarpetBG.Application.Interfaces.Services;

using Microsoft.AspNetCore.Mvc;

namespace CarpetBG.API.Controllers;

[ApiController]
[Route("api/order-items")]
public class OrderItemsController(IOrderItemService orderItemService) : ControllerBase
{


    //[HttpPost]
    ////[Authorize]
    //public async Task<IActionResult> AddOrderItem([FromBody] OrderItemDto dto)
    //{
    //    var result = await orderItemService.AddAsync(dto);
    //    return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    //}
}
