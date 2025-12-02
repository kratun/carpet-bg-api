using CarpetBG.Application.Interfaces.Services;

using Microsoft.AspNetCore.Mvc;

namespace CarpetBG.API.Controllers;

[ApiController]
//[Authorize]
[Route("api/products")]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await productService.GetAllAsync();

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
