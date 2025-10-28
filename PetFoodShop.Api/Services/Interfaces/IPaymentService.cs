using PetFoodShop.Api.Dtos;

namespace PetFoodShop.Api.Services.Interfaces;

public interface IPaymentService
{
    Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync();
    Task<PaymentDto?> GetPaymentByIdAsync(int id);
    Task<PaymentDto?> GetPaymentByOrderIdAsync(int orderId);
    Task<IEnumerable<PaymentDto>> GetPaymentsByStatusAsync(string status);
    Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto createDto);
    Task<PaymentDto?> UpdatePaymentAsync(int id, UpdatePaymentDto updateDto);
    Task<bool> DeletePaymentAsync(int id);
}

