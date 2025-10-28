using PetFoodShop.Api.Models;

namespace PetFoodShop.Api.Repositories.Interfaces;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<Cart?> GetCartByUserIdAsync(int userId);
    Task<Cart?> GetCartWithItemsAsync(int cartId);
    Task<Cartitem?> GetCartItemAsync(int cartItemId);
    Task<Cartitem> AddCartItemAsync(Cartitem cartItem);
    Task UpdateCartItemAsync(Cartitem cartItem);
    Task DeleteCartItemAsync(int cartItemId);
}

