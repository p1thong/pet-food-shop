using PetFoodShop.Api.Dtos;
using PetFoodShop.Api.Models;

namespace PetFoodShop.Api.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<IEnumerable<UserDto>> GetActiveUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    public Task<User?> GetUserByIdEntityAsync(int id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<UserDto> CreateUserAsync(CreateUserDto createDto);
    Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateDto);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> SoftDeleteUserAsync(int id);
}

