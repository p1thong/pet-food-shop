using PetFoodShop.Api.Dtos;

namespace PetFoodShop.Api.Services.Interfaces;

public interface IOrderService
{
    // Existing methods
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(int userId);
    Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto);
    Task<OrderDto?> UpdateOrderStatusAsync(int id, UpdateOrderDto updateDto);
    Task<bool> DeleteOrderAsync(int id);

    // New webhook-specific methods
    /// <summary>
    /// Update order status (used by webhook)
    /// </summary>
    Task UpdateOrderStatusAsync(int orderId, string status);

    /// <summary>
    /// Process order confirmation and deduct stock after successful payment
    /// </summary>
    Task ProcessOrderConfirmationAsync(int orderId);

    /// <summary>
    /// Cancel order and restore stock
    /// </summary>
    Task CancelOrderAsync(int orderId, string reason);
}