using Microsoft.EntityFrameworkCore;
using PetFoodShop.Api.Data;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Interfaces;

namespace PetFoodShop.Api.Repositories.Implements;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(PetFoodShopContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId)
    {
        return await _dbSet
            .Where(o => o.Userid == userId)
            .Include(o => o.Orderitems)
            .OrderByDescending(o => o.Placedat)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(o => o.User)
            .Include(o => o.Orderitems)
            .ThenInclude(oi => oi.Product)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status)
    {
        return await _dbSet
            .Where(o => o.Status == status)
            .Include(o => o.User)
            .Include(o => o.Orderitems)
            .OrderByDescending(o => o.Placedat)
            .ToListAsync();
    }
}

