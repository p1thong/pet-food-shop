namespace PetFoodShop.Api.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string? Fullname { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Role { get; set; } = null!;
    public bool Isdeleted { get; set; }
    public DateTime? Createdat { get; set; }
    public DateTime? Updatedat { get; set; }
}

public class CreateUserDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Fullname { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Role { get; set; } = "customer";
}

public class UpdateUserDto
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Fullname { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Role { get; set; }
    public bool? Isdeleted { get; set; }
}

