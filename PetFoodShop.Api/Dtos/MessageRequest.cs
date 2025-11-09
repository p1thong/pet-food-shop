namespace PetFoodShop.Api.Dtos
{
    public class MessageCreateRequest
    {
        public Guid ConversationId { get; set; }
        public int? OrderId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
