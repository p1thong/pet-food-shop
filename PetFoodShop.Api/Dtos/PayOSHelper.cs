namespace PetFoodShop.Api.Dtos;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public static class PayOSHelper
{
    /// <summary>
    /// Creates the signed JSON body for PayOS "Create Payment Link" request
    /// (replicates the Postman pre-request script you shared).
    /// </summary>
    public static string BuildSignedPaymentRequestBody(
        int amount,
        string cancelUrl,
        string description,
        string returnUrl,
        string checksumKey)
    {
        // 1️⃣ Generate unique orderCode and expiration timestamp
        long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long expiredAt = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds(); // +1 hour

        // 2️⃣ Build the string for signature
        // Must follow strict alphabet order:
        // amount=&cancelUrl=&description=&orderCode=&returnUrl=
        string sigRaw = $"amount={amount}" +
                        $"&cancelUrl={cancelUrl}" +
                        $"&description={description}" +
                        $"&orderCode={orderCode}" +
                        $"&returnUrl={returnUrl}";

        // 3️⃣ Compute HMAC SHA256 signature
        string signature = ComputeHmacSha256(sigRaw, checksumKey);

        // 4️⃣ Build the full request body
        var body = new
        {
            amount,
            cancelUrl,
            description,
            orderCode,
            returnUrl,
            expiredAt,
            signature
        };

        // 5️⃣ Serialize to JSON (same as Postman’s final body)
        return JsonSerializer.Serialize(body, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
    }

    private static string ComputeHmacSha256(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}
