namespace PetFoodShop.Api.Services.Interfaces
{
    public interface IFcmTokenService
    {
        public Task AddFcmTokenAsync(int userId, string token);
        public Task<List<string>> GetAllTokensAsync();
    }
}
