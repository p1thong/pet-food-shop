using PetFoodShop.Api.Models;

namespace PetFoodShop.Api.Repositories.Interfaces;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<Payment?> GetPaymentByOrderIdAsync(int orderId);
    Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status);
}

