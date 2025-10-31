using PetFoodShop.Api.Models;

namespace PetFoodShop.Api.Repositories.Interfaces
{
    public interface IFcmTokenRepository
    {
        public Task<Fcmtoken> GetByUserIdAsync(int userId);
    }
}
