using System;
using System.Collections.Generic;

namespace PetFoodShop.Api.Models;

public partial class Notification
{
    public int Id { get; set; }

    public int? Userid { get; set; }

    public string? Title { get; set; }

    public string? Body { get; set; }

    public string? Payload { get; set; }

    public DateTime? Sentat { get; set; }

    public virtual User? User { get; set; }
}
