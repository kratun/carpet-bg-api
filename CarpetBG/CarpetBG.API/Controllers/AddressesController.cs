using CarpetBG.Application.DTOs.Addresses;
using CarpetBG.Application.Interfaces.Services;

using Microsoft.AspNetCore.Mvc;

namespace CarpetBG.API.Controllers;

[ApiController]
[Route("api/addresses")]
public class AddressesController(IAddressService addressService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] AddressFilterDto filter)
    {
        var result = await addressService.GetFilteredAsync(filter);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await addressService.GetByIdAsync(id);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    //[Authorize]
    public async Task<IActionResult> Create([FromBody] CreateAddressDto dto)
    {
        var result = await addressService.CreateAddressAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
