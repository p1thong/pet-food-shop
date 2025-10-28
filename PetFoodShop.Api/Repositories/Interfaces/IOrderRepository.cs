using PetFoodShop.Api.Models;

namespace PetFoodShop.Api.Repositories.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId);
    Task<Order?> GetOrderWithDetailsAsync(int id);
    Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
}

