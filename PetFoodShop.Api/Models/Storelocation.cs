using System;
using System.Collections.Generic;

namespace PetFoodShop.Api.Models;

public partial class Storelocation
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? Address { get; set; }
}
