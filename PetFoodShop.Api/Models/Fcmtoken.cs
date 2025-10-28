using System;
using System.Collections.Generic;

namespace PetFoodShop.Api.Models;

public partial class Fcmtoken
{
    public int Id { get; set; }

    public int? Userid { get; set; }

    public string Token { get; set; } = null!;

    public string? Platform { get; set; }

    public DateTime? Createdat { get; set; }

    public virtual User? User { get; set; }
}
