namespace PetFoodShop.Api.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendWelcomeNotificationAsync(string fcmToken, string userName);
    }
}
