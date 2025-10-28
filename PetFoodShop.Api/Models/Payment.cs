using System;
using System.Collections.Generic;

namespace PetFoodShop.Api.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int? Orderid { get; set; }

    public string? Method { get; set; }

    public decimal? Amount { get; set; }

    public string? Status { get; set; }

    public string? Transactionid { get; set; }

    public DateTime? Paidat { get; set; }

    public DateTime? Createdat { get; set; }

    public virtual Order? Order { get; set; }
}
