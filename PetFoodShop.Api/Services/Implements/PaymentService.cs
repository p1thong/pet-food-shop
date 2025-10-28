using PetFoodShop.Api.Dtos;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Interfaces;
using PetFoodShop.Api.Services.Interfaces;

namespace PetFoodShop.Api.Services.Implements;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
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
        var payment = new Payment
        {
            Orderid = createDto.Orderid,
            Method = createDto.Method,
            Amount = createDto.Amount,
            Status = "pending",
            Transactionid = createDto.Transactionid,
            Createdat = DateTime.Now
        };

        var createdPayment = await _paymentRepository.AddAsync(payment);
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
        };
    }
}

