using FirebaseAdmin.Messaging;
using PetFoodShop.Api.Services.Interfaces;

namespace PetFoodShop.Api.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly FCMService _fcmService;

        public NotificationService(FCMService fcmService)
        {
            _fcmService = fcmService;
        }

        public async Task SendWelcomeNotificationAsync(string fcmToken, string userName)
        {
            try
            {
                if (string.IsNullOrEmpty(fcmToken))
                {
                    throw new ArgumentNullException(nameof(fcmToken));
                }

                var response = await _fcmService.SendToDeviceAsync(
                    fcmToken,
                    "🎉 Chào mừng đến với PetShop!",
                    $"Xin chào {userName}, cảm ơn bạn đã tham gia cùng chúng tôi 🐾",
                    "welcome",
                    new Dictionary<string, string>
                    {
                        { "type", "welcome" },
                        { "action", "open_home" }
                    }
                    );
                Console.WriteLine($"Sent welcome notification: {response}");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
