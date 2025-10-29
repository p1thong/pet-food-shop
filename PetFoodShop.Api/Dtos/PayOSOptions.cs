namespace PetFoodShop.Api.Dtos;

public sealed class PayOSOptions
{
    public string ClientId { get; set; } = default!;
    public string ApiKey { get; set; } = default!;
    public string ChecksumKey { get; set; } = default!;
    public string? PartnerCode { get; set; }   // optional
    public string ReturnUrl { get; set; } = default!;
    public string CancelUrl { get; set; } = default!;
}