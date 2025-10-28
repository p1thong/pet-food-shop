using System;
using System.Collections.Generic;

namespace PetFoodShop.Api.Models;

public partial class Cartitem
{
    public int Id { get; set; }

    public int? Cartid { get; set; }

    public int? Productid { get; set; }

    public int Quantity { get; set; }

    public decimal Pricesnapshot { get; set; }

    public DateTime? Addedat { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual Product? Product { get; set; }
}
