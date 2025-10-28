namespace PetFoodShop.Api.Dtos;

public class OrderDto
{
    public int Id { get; set; }
    public int? Userid { get; set; }
    public string? UserName { get; set; }
    public decimal Totalamount { get; set; }
    public string? Status { get; set; }
    public string? Shippingaddress { get; set; }
    public DateTime? Placedat { get; set; }
    public DateTime? Createdat { get; set; }
    public DateTime? Updatedat { get; set; }
    public List<OrderItemDto>? OrderItems { get; set; }
}

public class OrderItemDto
{
    public int Id { get; set; }
    public int? Productid { get; set; }
    public string? Productname { get; set; }
    public string? Productsku { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class CreateOrderDto
{
    public int Userid { get; set; }
    public string? Shippingaddress { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    public int Productid { get; set; }
    public int Quantity { get; set; }
}

public class UpdateOrderDto
{
    public string? Status { get; set; }
    public string? Shippingaddress { get; set; }
}

