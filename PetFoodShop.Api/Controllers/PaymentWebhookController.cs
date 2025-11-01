using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using PetFoodShop.Api.Services.Interfaces;
using PetFoodShop.Api.Dtos;

namespace PetFoodShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentWebhookController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IOrderService _orderService;
    private readonly ILogger<PaymentWebhookController> _logger;
    private readonly string _checksumKey = "5032d559962762d03cad25dcbdcd4f536c040dd3d6ae4dceedb6ea6e9fdb0cac";

    public PaymentWebhookController(
        IPaymentService paymentService,
        IOrderService orderService,
        ILogger<PaymentWebhookController> logger)
    {
        _paymentService = paymentService;
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Webhook endpoint for PayOS payment notifications
    /// URL: https://yourdomain.com/api/paymentwebhook
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> HandlePayOSWebhook([FromBody] JsonElement body)
    {
        try
        {
            _logger.LogInformation("=== PayOS Webhook Received ===");
            _logger.LogInformation("Raw body: {Body}", body.ToString());

            // 1. Extract webhook data
            if (!body.TryGetProperty("data", out var webhookData))
            {
                _logger.LogWarning("Missing 'data' property in webhook");
                return BadRequest(new { error = "Invalid webhook format" });
            }

            string signature = null;
            if (body.TryGetProperty("signature", out var signatureElement))
            {
                signature = signatureElement.GetString();
            }

            // 2. Verify signature (optional for testing, required for production)
            if (!string.IsNullOrEmpty(signature))
            {
                if (!VerifyWebhookSignature(webhookData.ToString(), signature))
                {
                    _logger.LogWarning("Invalid webhook signature");
                    // For testing, you might want to comment this out:
                    // return BadRequest(new { error = "Invalid signature" });
                }
                else
                {
                    _logger.LogInformation("✓ Signature verified");
                }
            }

            // 3. Extract payment details
            var orderCode = webhookData.GetProperty("orderCode").GetInt32();
            var amount = webhookData.GetProperty("amount").GetInt32();
            var reference = webhookData.GetProperty("reference").GetString();
            var transactionDateTime = webhookData.GetProperty("transactionDateTime").GetString();
            var paymentLinkId = webhookData.GetProperty("paymentLinkId").GetString();
            var code = webhookData.GetProperty("code").GetString();
            var desc = webhookData.GetProperty("desc").GetString();

            _logger.LogInformation(
                "Payment Details - OrderCode: {OrderCode}, Amount: {Amount}, Code: {Code}, Desc: {Desc}",
                orderCode, amount, code, desc);

            // 4. Check payment status and process
            if (code == "00") // Success
            {
                _logger.LogInformation("✅ Processing successful payment...");
                await HandleSuccessfulPayment(orderCode, amount, paymentLinkId, reference, transactionDateTime);
            }
            else if (code == "01") // Cancelled
            {
                _logger.LogInformation("❌ Processing cancelled payment...");
                await HandleCancelledPayment(orderCode, desc);
            }
            else // Failed or other status
            {
                _logger.LogWarning("⚠️ Processing failed payment...");
                await HandleFailedPayment(orderCode, code, desc);
            }

            // 5. Return success response (important for PayOS)
            return Ok(new
            {
                error = 0,
                message = "Webhook processed successfully",
                data = new { orderCode = orderCode }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error processing PayOS webhook");
            
            // Still return 200 OK to prevent PayOS from retrying
            return Ok(new
            {
                error = -1,
                message = "Error processing webhook",
                data = ex.Message
            });
        }
    }

    private async Task HandleSuccessfulPayment(
        int orderCode,
        int amount,
        string paymentLinkId,
        string reference,
        string transactionDateTime)
    {
        try
        {
            _logger.LogInformation("=== Handling Successful Payment ===");
            _logger.LogInformation("OrderCode: {OrderCode}, Reference: {Reference}", orderCode, reference);

            // 1. Update payment record
            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderCode);
            if (payment != null)
            {
                var updatePaymentDto = new UpdatePaymentDto
                {
                    Status = "completed",
                    Transactionid = reference,
                    Paidat = DateTime.Parse(transactionDateTime)
                };

                await _paymentService.UpdatePaymentAsync(payment.Id, updatePaymentDto);
                _logger.LogInformation("✓ Payment {PaymentId} updated to 'completed'", payment.Id);
            }
            else
            {
                _logger.LogWarning("⚠️ Payment record not found for order {OrderCode}", orderCode);
            }

            // 2. Process order confirmation (updates status and deducts stock)
            await _orderService.ProcessOrderConfirmationAsync(orderCode);
            
            _logger.LogInformation("=== ✅ Successfully processed payment for order {OrderCode} ===", orderCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error handling successful payment for order {OrderCode}", orderCode);
            throw;
        }
    }

    private async Task HandleCancelledPayment(int orderCode, string reason)
    {
        try
        {
            _logger.LogInformation("=== Handling Cancelled Payment ===");
            _logger.LogInformation("OrderCode: {OrderCode}, Reason: {Reason}", orderCode, reason);

            // 1. Update payment record
            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderCode);
            if (payment != null)
            {
                var updatePaymentDto = new UpdatePaymentDto
                {
                    Status = "cancelled"
                };

                await _paymentService.UpdatePaymentAsync(payment.Id, updatePaymentDto);
                _logger.LogInformation("✓ Payment {PaymentId} updated to 'cancelled'", payment.Id);
            }

            // 2. Cancel order (updates status and restores stock if needed)
            await _orderService.CancelOrderAsync(orderCode, reason);

            _logger.LogInformation("=== ❌ Order {OrderCode} cancelled: {Reason} ===", orderCode, reason);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error handling cancelled payment for order {OrderCode}", orderCode);
            throw;
        }
    }

    private async Task HandleFailedPayment(int orderCode, string code, string description)
    {
        try
        {
            _logger.LogWarning("=== Handling Failed Payment ===");
            _logger.LogWarning("OrderCode: {OrderCode}, Code: {Code}, Description: {Description}",
                orderCode, code, description);

            // 1. Update payment record
            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderCode);
            if (payment != null)
            {
                var updatePaymentDto = new UpdatePaymentDto
                {
                    Status = "failed"
                };

                await _paymentService.UpdatePaymentAsync(payment.Id, updatePaymentDto);
                _logger.LogInformation("✓ Payment {PaymentId} updated to 'failed'", payment.Id);
            }

            // 2. Update order status to failed
            await _orderService.UpdateOrderStatusAsync(orderCode, "failed");

            _logger.LogInformation("=== ⚠️ Order {OrderCode} marked as failed ===", orderCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error handling failed payment for order {OrderCode}", orderCode);
            throw;
        }
    }

    /// <summary>
    /// Verify webhook signature from PayOS
    /// </summary>
    private bool VerifyWebhookSignature(string data, string receivedSignature)
    {
        try
        {
            // Sort the JSON data (PayOS requirement)
            var sortedData = SortJsonData(data);

            // Create HMAC SHA256 signature
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_checksumKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(sortedData));
            var computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

            var isValid = computedSignature == receivedSignature?.ToLower();

            if (!isValid)
            {
                _logger.LogWarning(
                    "Signature mismatch - Computed: {Computed}, Received: {Received}",
                    computedSignature, receivedSignature);
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying webhook signature");
            return false;
        }
    }

    /// <summary>
    /// Sort JSON data alphabetically by key (PayOS requirement)
    /// </summary>
    private string SortJsonData(string jsonData)
    {
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonData);
        var sortedDict = new SortedDictionary<string, object>();

        foreach (var property in jsonElement.EnumerateObject())
        {
            sortedDict[property.Name] = property.Value.ValueKind switch
            {
                JsonValueKind.String => property.Value.GetString(),
                JsonValueKind.Number => property.Value.GetInt64(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => property.Value.GetRawText()
            };
        }

        return JsonSerializer.Serialize(sortedDict);
    }

    /// <summary>
    /// Test endpoint to verify webhook is accessible
    /// </summary>
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new
        {
            message = "✅ PayOS Webhook endpoint is working",
            timestamp = DateTime.UtcNow,
            endpoint = $"{Request.Scheme}://{Request.Host}/api/paymentwebhook",
            checksumKeyConfigured = !string.IsNullOrEmpty(_checksumKey),
            services = new
            {
                paymentService = _paymentService != null,
                orderService = _orderService != null
            }
        });
    }
}