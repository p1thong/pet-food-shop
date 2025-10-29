namespace PetFoodShop.Api.Dtos;

public class PaymentDto
{
    public int Id { get; set; }
    public int? Orderid { get; set; }
    public string? Method { get; set; }
    public decimal? Amount { get; set; }
    public string? Status { get; set; }
    public string? Transactionid { get; set; }
    public DateTime? Paidat { get; set; }
    public DateTime? Createdat { get; set; }
    public string? PaymentLink { get; set; }
}

public class CreatePaymentDto
{
    public int Orderid { get; set; }
    public string Method { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? Transactionid { get; set; }
}

public class UpdatePaymentDto
{
    public string? Status { get; set; }
    public string? Transactionid { get; set; }
    public DateTime? Paidat { get; set; }
}

