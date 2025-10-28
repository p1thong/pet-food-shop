using Microsoft.EntityFrameworkCore;
using PetFoodShop.Api.Data;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Interfaces;

namespace PetFoodShop.Api.Repositories.Implements;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(PetFoodShopContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Where(p => p.Categoryid == categoryId && !p.Isdeleted)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        return await _dbSet
            .Where(p => !p.Isdeleted)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<Product?> GetProductWithCategoryAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}

