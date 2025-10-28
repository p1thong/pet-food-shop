namespace PetFoodShop.Api.Dtos;

public class CartDto
{
    public int Id { get; set; }
    public int? Userid { get; set; }
    public DateTime? Createdat { get; set; }
    public DateTime? Updatedat { get; set; }
    public List<CartItemDto>? CartItems { get; set; }
    public decimal TotalAmount { get; set; }
}

public class CartItemDto
{
    public int Id { get; set; }
    public int? Productid { get; set; }
    public string? ProductName { get; set; }
    public string? ProductSku { get; set; }
    public string? ProductImage { get; set; }
    public int Quantity { get; set; }
    public decimal Pricesnapshot { get; set; }
    public DateTime? Addedat { get; set; }
}

public class AddToCartDto
{
    public int Userid { get; set; }
    public int Productid { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UpdateCartItemDto
{
    public int Quantity { get; set; }
}

