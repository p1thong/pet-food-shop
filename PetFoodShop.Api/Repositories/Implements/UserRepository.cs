using Microsoft.EntityFrameworkCore;
using PetFoodShop.Api.Data;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Interfaces;

namespace PetFoodShop.Api.Repositories.Implements;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(PetFoodShopContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _dbSet
            .Where(u => !u.Isdeleted)
            .ToListAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }
}

