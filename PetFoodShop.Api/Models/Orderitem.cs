using System;
using System.Collections.Generic;

namespace PetFoodShop.Api.Models;

public partial class Orderitem
{
    public int Id { get; set; }

    public int? Orderid { get; set; }

    public int? Productid { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public string? Productname { get; set; }

    public string? Productsku { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
