namespace PetFoodShop.Api.Dtos;

public class ProductDto
{
    public int Id { get; set; }
    public string? Sku { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int? Stock { get; set; }
    public int? Categoryid { get; set; }
    public string? CategoryName { get; set; }
    public string? Imageurl { get; set; }
    public bool Isdeleted { get; set; }
    public DateTime? Createdat { get; set; }
    public DateTime? Updatedat { get; set; }
}

public class CreateProductDto
{
    public string? Sku { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int? Stock { get; set; }
    public int? Categoryid { get; set; }
    public string? Imageurl { get; set; }
}

public class UpdateProductDto
{
    public string? Sku { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? Stock { get; set; }
    public int? Categoryid { get; set; }
    public string? Imageurl { get; set; }
    public bool? Isdeleted { get; set; }
}

