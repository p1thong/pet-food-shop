using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Implements;
using PetFoodShop.Api.Services.Interfaces;

namespace PetFoodShop.Api.Services.Implements
{
    public class MessageService : IMessageService
    {
        private readonly MessageRepository _repository;

        public MessageService(MessageRepository repository)
        {
            _repository = repository;
        }

        public async Task SaveNewMessage(Message message)
        {
            try
            {
                await _repository.AddAsync(message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
