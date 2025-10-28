using PetFoodShop.Api.Models;

namespace PetFoodShop.Api.Repositories.Interfaces;

public interface IStoreLocationRepository : IGenericRepository<Storelocation>
{
    Task<IEnumerable<Storelocation>> GetNearbyLocationsAsync(double latitude, double longitude, double radiusKm);
}

