using System;
using System.Collections.Generic;

namespace PetFoodShop.Api.Models;

public partial class Order
{
    public int Id { get; set; }

    public int? Userid { get; set; }

    public decimal Totalamount { get; set; }

    public string? Status { get; set; }

    public string? Shippingaddress { get; set; }

    public DateTime? Placedat { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();

    public virtual Payment? Payment { get; set; }

    public virtual User? User { get; set; }
}
