using Microsoft.AspNetCore.Mvc;
using PetFoodShop.Api.Dtos;
using PetFoodShop.Api.Services.Interfaces;

namespace PetFoodShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoreLocationsController : ControllerBase
{
    private readonly IStoreLocationService _storeLocationService;

    public StoreLocationsController(IStoreLocationService storeLocationService)
    {
        _storeLocationService = storeLocationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StoreLocationDto>>> GetAll()
    {
        var locations = await _storeLocationService.GetAllLocationsAsync();
        return Ok(locations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StoreLocationDto>> GetById(int id)
    {
        var location = await _storeLocationService.GetLocationByIdAsync(id);
        if (location == null)
            return NotFound(new { message = "Store location not found" });

        return Ok(location);
    }

    [HttpGet("nearby")]
    public async Task<ActionResult<IEnumerable<StoreLocationDto>>> GetNearby(
        [FromQuery] double latitude, 
        [FromQuery] double longitude, 
        [FromQuery] double radiusKm = 10)
    {
        var locations = await _storeLocationService.GetNearbyLocationsAsync(latitude, longitude, radiusKm);
        return Ok(locations);
    }

    [HttpPost]
    public async Task<ActionResult<StoreLocationDto>> Create([FromBody] CreateStoreLocationDto createDto)
    {
        var location = await _storeLocationService.CreateLocationAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = location.Id }, location);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<StoreLocationDto>> Update(int id, [FromBody] UpdateStoreLocationDto updateDto)
    {
        var location = await _storeLocationService.UpdateLocationAsync(id, updateDto);
        if (location == null)
            return NotFound(new { message = "Store location not found" });

        return Ok(location);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _storeLocationService.DeleteLocationAsync(id);
        if (!result)
            return NotFound(new { message = "Store location not found" });

        return NoContent();
    }
}

