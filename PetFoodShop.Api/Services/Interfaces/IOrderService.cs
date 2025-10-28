using PetFoodShop.Api.Dtos;

namespace PetFoodShop.Api.Services.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(int userId);
    Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto);
    Task<OrderDto?> UpdateOrderStatusAsync(int id, UpdateOrderDto updateDto);
    Task<bool> DeleteOrderAsync(int id);
}

