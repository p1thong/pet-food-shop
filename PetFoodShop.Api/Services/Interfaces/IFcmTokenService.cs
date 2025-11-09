using PetFoodShop.Api.Models;

namespace PetFoodShop.Api.Services.Interfaces
{
    public interface IFcmTokenService
    {
        public Task AddFcmTokenAsync(int userId, string token);
        public Task<Fcmtoken> GetTokenByUserIdAsync(int userId);
        public Task<List<string>> GetAllTokensAsync();
    }
}
