using Microsoft.EntityFrameworkCore;
using PetFoodShop.Api.Data;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Interfaces;

namespace PetFoodShop.Api.Repositories.Implements;

public class CartRepository : GenericRepository<Cart>, ICartRepository
{
    public CartRepository(PetFoodShopContext context) : base(context)
    {
    }

    public async Task<Cart?> GetCartByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(c => c.Cartitems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.Userid == userId);
    }

    public async Task<Cart?> GetCartWithItemsAsync(int cartId)
    {
        return await _dbSet
            .Include(c => c.Cartitems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.Id == cartId);
    }

    public async Task<Cartitem?> GetCartItemAsync(int cartItemId)
    {
        return await _context.Cartitems
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
    }

    public async Task<Cartitem> AddCartItemAsync(Cartitem cartItem)
    {
        await _context.Cartitems.AddAsync(cartItem);
        await _context.SaveChangesAsync();
        return cartItem;
    }

    public async Task UpdateCartItemAsync(Cartitem cartItem)
    {
        _context.Cartitems.Update(cartItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCartItemAsync(int cartItemId)
    {
        var cartItem = await _context.Cartitems.FindAsync(cartItemId);
        if (cartItem != null)
        {
            _context.Cartitems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }
    }
}

