using Microsoft.EntityFrameworkCore;
using PetFoodShop.Api.Data;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Interfaces;

namespace PetFoodShop.Api.Repositories.Implements;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(PetFoodShopContext context) : base(context)
    {
    }

    public async Task<Category?> GetCategoryWithProductsAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Products.Where(p => !p.Isdeleted))
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}

