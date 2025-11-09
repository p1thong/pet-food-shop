using PetFoodShop.Api.Data;
using PetFoodShop.Api.Models;

namespace PetFoodShop.Api.Repositories.Implements
{
    public class MessageRepository : GenericRepository<Message>
    {
        public MessageRepository(PetFoodShopContext context) : base(context)
        {
        }
    }
}
