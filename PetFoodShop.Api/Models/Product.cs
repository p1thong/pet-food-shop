using System;
using System.Collections.Generic;

namespace PetFoodShop.Api.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Sku { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int? Stock { get; set; }

    public int? Categoryid { get; set; }

    public string? Imageurl { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<Cartitem> Cartitems { get; set; } = new List<Cartitem>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();
}
