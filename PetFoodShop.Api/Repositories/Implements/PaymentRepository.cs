using Microsoft.EntityFrameworkCore;
using PetFoodShop.Api.Data;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Interfaces;

namespace PetFoodShop.Api.Repositories.Implements;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(PetFoodShopContext context) : base(context)
    {
    }

    public async Task<Payment?> GetPaymentByOrderIdAsync(int orderId)
    {
        return await _dbSet
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Orderid == orderId);
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status)
    {
        return await _dbSet
            .Where(p => p.Status == status)
            .Include(p => p.Order)
            .ToListAsync();
    }
}

