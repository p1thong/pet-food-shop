using PetFoodShop.Api.Dtos;

namespace PetFoodShop.Api.Services.Interfaces;

public interface ICartService
{
    Task<CartDto?> GetCartByUserIdAsync(int userId);
    Task<CartDto> AddToCartAsync(AddToCartDto addToCartDto);
    Task<CartDto?> UpdateCartItemAsync(int cartItemId, UpdateCartItemDto updateDto);
    Task<bool> RemoveFromCartAsync(int cartItemId);
    Task<bool> ClearCartAsync(int userId);
}

