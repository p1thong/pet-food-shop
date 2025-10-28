using Microsoft.AspNetCore.Mvc;
using PetFoodShop.Api.Dtos;
using PetFoodShop.Api.Services.Interfaces;

namespace PetFoodShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartsController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartsController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<CartDto>> GetCartByUserId(int userId)
    {
        var cart = await _cartService.GetCartByUserIdAsync(userId);
        if (cart == null)
            return NotFound(new { message = "Cart not found" });

        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddToCart([FromBody] AddToCartDto addToCartDto)
    {
        try
        {
            var cart = await _cartService.AddToCartAsync(addToCartDto);
            return Ok(cart);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("items/{cartItemId}")]
    public async Task<ActionResult<CartDto>> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemDto updateDto)
    {
        var cart = await _cartService.UpdateCartItemAsync(cartItemId, updateDto);
        if (cart == null)
            return NotFound(new { message = "Cart item not found" });

        return Ok(cart);
    }

    [HttpDelete("items/{cartItemId}")]
    public async Task<ActionResult> RemoveFromCart(int cartItemId)
    {
        var result = await _cartService.RemoveFromCartAsync(cartItemId);
        if (!result)
            return NotFound(new { message = "Cart item not found" });

        return NoContent();
    }

    [HttpDelete("user/{userId}/clear")]
    public async Task<ActionResult> ClearCart(int userId)
    {
        var result = await _cartService.ClearCartAsync(userId);
        if (!result)
            return NotFound(new { message = "Cart not found" });

        return NoContent();
    }
}

