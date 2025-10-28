using Microsoft.EntityFrameworkCore;
using PetFoodShop.Api.Data;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Interfaces;

namespace PetFoodShop.Api.Repositories.Implements;

public class StoreLocationRepository : GenericRepository<Storelocation>, IStoreLocationRepository
{
    public StoreLocationRepository(PetFoodShopContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Storelocation>> GetNearbyLocationsAsync(double latitude, double longitude, double radiusKm)
    {
        // Simple distance calculation using Haversine formula approximation
        var locations = await _dbSet.ToListAsync();
        
        return locations.Where(loc => 
        {
            if (!loc.Latitude.HasValue || !loc.Longitude.HasValue)
                return false;
                
            var distance = CalculateDistance(latitude, longitude, loc.Latitude.Value, loc.Longitude.Value);
            return distance <= radiusKm;
        }).ToList();
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in km
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}

