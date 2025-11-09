using PetFoodShop.Api.Models;

namespace PetFoodShop.Api.Services.Interfaces
{
    public interface IMessageService 
    {
        Task SaveNewMessage(Message message);
    }
}
