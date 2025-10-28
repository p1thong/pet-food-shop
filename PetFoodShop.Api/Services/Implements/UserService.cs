using PetFoodShop.Api.Dtos;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Interfaces;
using PetFoodShop.Api.Services.Interfaces;

namespace PetFoodShop.Api.Services.Implements;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<IEnumerable<UserDto>> GetActiveUsersAsync()
    {
        var users = await _userRepository.GetActiveUsersAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createDto)
    {
        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(createDto.Email))
        {
            throw new InvalidOperationException("Email already exists");
        }

        // Hash password (simple implementation - consider using BCrypt in production)
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(createDto.Password);

        var user = new User
        {
            Email = createDto.Email,
            Password = hashedPassword,
            Fullname = createDto.Fullname,
            Phone = createDto.Phone,
            Address = createDto.Address,
            Role = createDto.Role,
            Isdeleted = false,
            Createdat = DateTime.Now,
            Updatedat = DateTime.Now
        };

        var createdUser = await _userRepository.AddAsync(user);
        return MapToDto(createdUser);
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        if (updateDto.Email != null) user.Email = updateDto.Email;
        if (updateDto.Password != null) user.Password = BCrypt.Net.BCrypt.HashPassword(updateDto.Password);
        if (updateDto.Fullname != null) user.Fullname = updateDto.Fullname;
        if (updateDto.Phone != null) user.Phone = updateDto.Phone;
        if (updateDto.Address != null) user.Address = updateDto.Address;
        if (updateDto.Role != null) user.Role = updateDto.Role;
        if (updateDto.Isdeleted.HasValue) user.Isdeleted = updateDto.Isdeleted.Value;
        user.Updatedat = DateTime.Now;

        await _userRepository.UpdateAsync(user);
        return MapToDto(user);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var exists = await _userRepository.ExistsAsync(id);
        if (!exists) return false;

        await _userRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> SoftDeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return false;

        user.Isdeleted = true;
        user.Updatedat = DateTime.Now;
        await _userRepository.UpdateAsync(user);
        return true;
    }

    private UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Fullname = user.Fullname,
            Phone = user.Phone,
            Address = user.Address,
            Role = user.Role,
            Isdeleted = user.Isdeleted,
            Createdat = user.Createdat,
            Updatedat = user.Updatedat
        };
    }
}

