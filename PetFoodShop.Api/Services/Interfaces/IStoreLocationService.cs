using PetFoodShop.Api.Dtos;

namespace PetFoodShop.Api.Services.Interfaces;

public interface IStoreLocationService
{
    Task<IEnumerable<StoreLocationDto>> GetAllLocationsAsync();
    Task<StoreLocationDto?> GetLocationByIdAsync(int id);
    Task<IEnumerable<StoreLocationDto>> GetNearbyLocationsAsync(double latitude, double longitude, double radiusKm);
    Task<StoreLocationDto> CreateLocationAsync(CreateStoreLocationDto createDto);
    Task<StoreLocationDto?> UpdateLocationAsync(int id, UpdateStoreLocationDto updateDto);
    Task<bool> DeleteLocationAsync(int id);
}

