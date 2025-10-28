using System;
using System.Collections.Generic;

namespace PetFoodShop.Api.Models;

public partial class Message
{
    public int Id { get; set; }

    public Guid Conversationid { get; set; }

    public int? Orderid { get; set; }

    public int Senderid { get; set; }

    public int Receiverid { get; set; }

    public string? Message1 { get; set; }

    public DateTime? Createdat { get; set; }

    public bool? Isread { get; set; }

    public virtual Order? Order { get; set; }

    public virtual User Receiver { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
