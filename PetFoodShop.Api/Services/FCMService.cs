using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace PetFoodShop.Api.Services
{
    public class FCMService
    {
        private readonly ILogger<FCMService> _logger;

        public FCMService(ILogger<FCMService> logger)
        {
            _logger = logger;
            if (FirebaseApp.DefaultInstance == null)
            {
                var firebasePath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS") ?? "fcmadmin.json";
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(firebasePath)
                });
            }
        }

        public async Task<string> SendToDeviceAsync(
            string token,
            string title,
            string body,
            string type,
            Dictionary<string, string>? data = null)
        {
            var message = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = new Dictionary<string, string>(data ?? new Dictionary<string, string>())
                {
                    ["type"] = type
                },
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        Sound = "default"
                    }
                }
            };
            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            _logger.LogInformation("Sent FCM message: {Response}", response);
            return response;
        }
    }
}
