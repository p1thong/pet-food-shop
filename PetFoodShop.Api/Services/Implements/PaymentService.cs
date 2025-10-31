using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PayOS;
using PetFoodShop.Api.Dtos;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Interfaces;
using PetFoodShop.Api.Services.Interfaces;
using PayOS.Models;
using PayOS.Models.V2.PaymentRequests;
using PayOSOptions = PetFoodShop.Api.Dtos.PayOSOptions;

namespace PetFoodShop.Api.Services.Implements;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly PayOSClient _payos; // v2 client
    private readonly PayOSOptions _opt;

    public PaymentService(
        IPaymentRepository paymentRepository,
        PayOSClient payos,
        IOptions<PayOSOptions> opt)
    {
        _paymentRepository = paymentRepository;
        _payos = payos;
        _opt = opt.Value;
    }

    public async Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync()
    {
        var payments = await _paymentRepository.GetAllAsync();
        return payments.Select(MapToDto);
    }

    public async Task<PaymentDto?> GetPaymentByIdAsync(int id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        return payment == null ? null : MapToDto(payment);
    }

    public async Task<PaymentDto?> GetPaymentByOrderIdAsync(int orderId)
    {
        var payment = await _paymentRepository.GetPaymentByOrderIdAsync(orderId);
        return payment == null ? null : MapToDto(payment);
    }

    public async Task<IEnumerable<PaymentDto>> GetPaymentsByStatusAsync(string status)
    {
        var payments = await _paymentRepository.GetPaymentsByStatusAsync(status);
        return payments.Select(MapToDto);
    }

    public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createDto)
    {
        using var http = new HttpClient();
        http.BaseAddress = new Uri("https://api-merchant.payos.vn");

        // Use deep links for mobile app
        var json = PayOSHelper.BuildSignedPaymentRequestBody(
            amount: createDto.Amount,
            cancelUrl: "petshop://payment/cancel",  // Deep link for cancel
            description: $"Order #{createDto.Orderid}",
            returnUrl: "petshop://payment/success", // Deep link for success
            checksumKey: "5032d559962762d03cad25dcbdcd4f536c040dd3d6ae4dceedb6ea6e9fdb0cac"
        );

        var req = new HttpRequestMessage(HttpMethod.Post, "/v2/payment-requests")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        // Add headers
        req.Headers.Add("x-client-id", "504291a5-3666-4475-8e83-69483fb1858e");
        req.Headers.Add("x-api-key", "281d483e-823a-4c19-8eb6-4a242486850c");

        var resp = await http.SendAsync(req);
        string respBody = await resp.Content.ReadAsStringAsync();
        
        using var doc = JsonDocument.Parse(respBody);
        string checkoutUrl = doc.RootElement
            .GetProperty("data")
            .GetProperty("checkoutUrl")
            .GetString();

        // Save the payment record (after getting checkout URL)
        var payment = new Payment
        {
            Orderid       = createDto.Orderid,
            Method        = createDto.Method,
            Amount        = createDto.Amount,
            Status        = "pending",
            Transactionid = createDto.Transactionid, // store PayOS payment link id
            Paymentlink   = checkoutUrl,   // assign checkout URL here
            Createdat     = DateTime.Now
        };

        var createdPayment = await _paymentRepository.AddAsync(payment);

        // Return DTO to client
        return MapToDto(createdPayment);
    }

    public async Task<PaymentDto?> UpdatePaymentAsync(int id, UpdatePaymentDto updateDto)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null) return null;

        if (updateDto.Status != null) payment.Status = updateDto.Status;
        if (updateDto.Transactionid != null) payment.Transactionid = updateDto.Transactionid;
        if (updateDto.Paidat.HasValue) payment.Paidat = updateDto.Paidat.Value;

        await _paymentRepository.UpdateAsync(payment);
        return MapToDto(payment);
    }

    public async Task<bool> DeletePaymentAsync(int id)
    {
        var exists = await _paymentRepository.ExistsAsync(id);
        if (!exists) return false;

        await _paymentRepository.DeleteAsync(id);
        return true;
    }

    private PaymentDto MapToDto(Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            Orderid = payment.Orderid,
            Method = payment.Method,
            Amount = payment.Amount,
            Status = payment.Status,
            Transactionid = payment.Transactionid,
            Paidat = payment.Paidat,
            Createdat = payment.Createdat,
            PaymentLink = payment.Paymentlink
        };
    }
}