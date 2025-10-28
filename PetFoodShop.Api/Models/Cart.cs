using System;
using System.Collections.Generic;

namespace PetFoodShop.Api.Models;

public partial class Cart
{
    public int Id { get; set; }

    public int? Userid { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<Cartitem> Cartitems { get; set; } = new List<Cartitem>();

    public virtual User? User { get; set; }
}
