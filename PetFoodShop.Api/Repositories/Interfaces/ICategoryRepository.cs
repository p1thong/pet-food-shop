using PetFoodShop.Api.Models;

namespace PetFoodShop.Api.Repositories.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetCategoryWithProductsAsync(int id);
}

