using PetFoodShop.Api.Dtos;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Interfaces;
using PetFoodShop.Api.Services.Interfaces;

namespace PetFoodShop.Api.Services.Implements;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(MapToDto);
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var order = await _orderRepository.GetOrderWithDetailsAsync(id);
        return order == null ? null : MapToDto(order);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(int userId)
    {
        var orders = await _orderRepository.GetOrdersByUserAsync(userId);
        return orders.Select(MapToDto);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status)
    {
        var orders = await _orderRepository.GetOrdersByStatusAsync(status);
        return orders.Select(MapToDto);
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto)
    {
        decimal totalAmount = 0;
        var orderItems = new List<Orderitem>();

        foreach (var item in createDto.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.Productid);
            if (product == null)
                throw new InvalidOperationException($"Product with ID {item.Productid} not found");

            if (product.Stock < item.Quantity)
                throw new InvalidOperationException($"Insufficient stock for product {product.Name}");

            var orderItem = new Orderitem
            {
                Productid = item.Productid,
                Productname = product.Name,
                Productsku = product.Sku,
                Quantity = item.Quantity,
                Price = product.Price
            };

            totalAmount += product.Price * item.Quantity;
            orderItems.Add(orderItem);

            // Update stock
            product.Stock -= item.Quantity;
            await _productRepository.UpdateAsync(product);
        }

        var order = new Order
        {
            Userid = createDto.Userid,
            Totalamount = totalAmount,
            Status = "pending",
            Shippingaddress = createDto.Shippingaddress,
            Placedat = DateTime.Now,
            Createdat = DateTime.Now,
            Updatedat = DateTime.Now,
            Orderitems = orderItems
        };

        var createdOrder = await _orderRepository.AddAsync(order);
        return MapToDto(createdOrder);
    }

    public async Task<OrderDto?> UpdateOrderStatusAsync(int id, UpdateOrderDto updateDto)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) return null;

        if (updateDto.Status != null) order.Status = updateDto.Status;
        if (updateDto.Shippingaddress != null) order.Shippingaddress = updateDto.Shippingaddress;
        order.Updatedat = DateTime.Now;

        await _orderRepository.UpdateAsync(order);
        return MapToDto(order);
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var exists = await _orderRepository.ExistsAsync(id);
        if (!exists) return false;

        await _orderRepository.DeleteAsync(id);
        return true;
    }

    private OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            Userid = order.Userid,
            UserName = order.User?.Fullname,
            Totalamount = order.Totalamount,
            Status = order.Status,
            Shippingaddress = order.Shippingaddress,
            Placedat = order.Placedat,
            Createdat = order.Createdat,
            Updatedat = order.Updatedat,
            OrderItems = order.Orderitems?.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                Productid = oi.Productid,
                Productname = oi.Productname,
                Productsku = oi.Productsku,
                Quantity = oi.Quantity,
                Price = oi.Price
            }).ToList()
        };
    }
}

