using Microsoft.EntityFrameworkCore;
using PetFoodShop.Api.Data;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Interfaces;

namespace PetFoodShop.Api.Repositories.Implements
{
    public class FcmRepository : GenericRepository<Fcmtoken>, IFcmTokenRepository
    {
        public FcmRepository(PetFoodShopContext context) : base(context)
        {
        }

        public async Task<Fcmtoken> GetByUserIdAsync(int userId)
        {
            return await _context.Fcmtokens.FirstOrDefaultAsync(t => t.Userid == userId);
        }
    }
}
