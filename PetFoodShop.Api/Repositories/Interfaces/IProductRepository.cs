using PetFoodShop.Api.Models;

namespace PetFoodShop.Api.Repositories.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> GetActiveProductsAsync();
    Task<Product?> GetProductWithCategoryAsync(int id);
}

