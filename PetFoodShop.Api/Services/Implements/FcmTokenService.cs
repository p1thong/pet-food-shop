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
    }
}
