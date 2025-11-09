using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Implements;
using PetFoodShop.Api.Repositories.Interfaces;
using PetFoodShop.Api.Services.Interfaces;

namespace PetFoodShop.Api.Services.Implements
{
    public class FcmTokenService : IFcmTokenService
    {
        private readonly FcmRepository _fcmTokenRepository;

        public FcmTokenService(FcmRepository fcmTokenRepository)
        {
            _fcmTokenRepository = fcmTokenRepository;
        }

        public async Task AddFcmTokenAsync(int userId, string token)
        {
            try
            {
                var existToken = await _fcmTokenRepository.GetByUserIdAsync(userId);
                if (existToken != null)
                {
                    existToken.Token = token;
                    existToken.Createdat = DateTime.Now;
                    await _fcmTokenRepository.UpdateAsync(existToken);
                    return;
                }

                var fcmToken = new Fcmtoken
                {
                    Userid = userId,
                    Token = token,
                    Platform = "FCM",
                    Createdat = DateTime.Now
                };

                await _fcmTokenRepository.AddAsync(fcmToken);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<string>> GetAllTokensAsync()
        {
            try
            {
                var fcmTokens = await _fcmTokenRepository.GetAllAsync();
                var tokens = fcmTokens
                    .Select(t => t.Token)
                    .Where(t => !string.IsNullOrEmpty(t))
                    .Distinct()
                    .ToList();

                return tokens;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<Fcmtoken> GetTokenByUserIdAsync(int userId)
        {
            try
            {
                var token = _fcmTokenRepository.GetByUserIdAsync(userId);
                return token;
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
