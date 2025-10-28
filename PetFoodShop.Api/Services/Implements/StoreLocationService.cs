using PetFoodShop.Api.Dtos;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Interfaces;
using PetFoodShop.Api.Services.Interfaces;

namespace PetFoodShop.Api.Services.Implements;

public class StoreLocationService : IStoreLocationService
{
    private readonly IStoreLocationRepository _storeLocationRepository;

    public StoreLocationService(IStoreLocationRepository storeLocationRepository)
    {
        _storeLocationRepository = storeLocationRepository;
    }

    public async Task<IEnumerable<StoreLocationDto>> GetAllLocationsAsync()
    {
        var locations = await _storeLocationRepository.GetAllAsync();
        return locations.Select(MapToDto);
    }

    public async Task<StoreLocationDto?> GetLocationByIdAsync(int id)
    {
        var location = await _storeLocationRepository.GetByIdAsync(id);
        return location == null ? null : MapToDto(location);
    }

    public async Task<IEnumerable<StoreLocationDto>> GetNearbyLocationsAsync(double latitude, double longitude, double radiusKm)
    {
        var locations = await _storeLocationRepository.GetNearbyLocationsAsync(latitude, longitude, radiusKm);
        return locations.Select(MapToDto);
    }

    public async Task<StoreLocationDto> CreateLocationAsync(CreateStoreLocationDto createDto)
    {
        var location = new Storelocation
        {
            Name = createDto.Name,
            Latitude = createDto.Latitude,
            Longitude = createDto.Longitude,
            Address = createDto.Address
        };

        var createdLocation = await _storeLocationRepository.AddAsync(location);
        return MapToDto(createdLocation);
    }

    public async Task<StoreLocationDto?> UpdateLocationAsync(int id, UpdateStoreLocationDto updateDto)
    {
        var location = await _storeLocationRepository.GetByIdAsync(id);
        if (location == null)
        {
            return null;
        }

        location.Name = updateDto.Name;
        location.Latitude = updateDto.Latitude;
        location.Longitude = updateDto.Longitude;
        location.Address = updateDto.Address;

        await _storeLocationRepository.UpdateAsync(location);
        return MapToDto(location);
    }

    public async Task<bool> DeleteLocationAsync(int id)
    {
        var exists = await _storeLocationRepository.ExistsAsync(id);
        if (!exists) return false;

        await _storeLocationRepository.DeleteAsync(id);
        return true;
    }

    private StoreLocationDto MapToDto(Storelocation location)
    {
        return new StoreLocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Address = location.Address
        };
    }
}

