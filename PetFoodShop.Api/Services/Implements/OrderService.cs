using PetFoodShop.Api.Dtos;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Interfaces;
using PetFoodShop.Api.Services.Interfaces;

namespace PetFoodShop.Api.Services.Implements;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository, 
        IProductRepository productRepository,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _logger = logger;
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

            // Don't update stock here - will be done by webhook when payment is confirmed
            // product.Stock -= item.Quantity;
            // await _productRepository.UpdateAsync(product);
        }

        var order = new Order
        {
            Userid = createDto.Userid,
            Totalamount = totalAmount,
            Status = "pending", // Will be updated to "paid" by webhook
            Shippingaddress = createDto.Shippingaddress,
            Placedat = DateTime.Now,
            Createdat = DateTime.Now,
            Updatedat = DateTime.Now,
            Orderitems = orderItems
        };

        var createdOrder = await _orderRepository.AddAsync(order);
        
        _logger.LogInformation(
            "Order created - OrderId: {OrderId}, UserId: {UserId}, Total: {Total}, Status: {Status}",
            createdOrder.Id, createdOrder.Userid, createdOrder.Totalamount, createdOrder.Status);
        
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
        
        _logger.LogInformation(
            "Order status updated - OrderId: {OrderId}, Status: {Status}",
            order.Id, order.Status);
        
        return MapToDto(order);
    }

    /// <summary>
    /// Update order status by order ID (used by webhook)
    /// </summary>
    public async Task UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found for status update", orderId);
            return;
        }

        order.Status = status;
        order.Updatedat = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(order);
        
        _logger.LogInformation(
            "Order status updated via webhook - OrderId: {OrderId}, Status: {Status}",
            orderId, status);
    }

    /// <summary>
    /// Process order confirmation after successful payment (called by webhook)
    /// </summary>
    public async Task ProcessOrderConfirmationAsync(int orderId)
    {
        _logger.LogInformation("Processing order confirmation for OrderId: {OrderId}", orderId);

        var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", orderId);
            return;
        }

        // Update order status to paid
        order.Status = "paid";
        order.Updatedat = DateTime.UtcNow;

        // Deduct stock for each order item
        if (order.Orderitems != null && order.Orderitems.Any())
        {
            _logger.LogInformation(
                "Deducting stock for {Count} items in order {OrderId}",
                order.Orderitems.Count, orderId);

            foreach (var item in order.Orderitems)
            {
                if (item.Productid.HasValue)
                {
                    var product = await _productRepository.GetByIdAsync(item.Productid.Value);
                    if (product != null)
                    {
                        var oldStock = product.Stock;
                        product.Stock -= item.Quantity;
                        product.Updatedat = DateTime.UtcNow;
                        
                        await _productRepository.UpdateAsync(product);
                        
                        _logger.LogInformation(
                            "Stock deducted - Product: {ProductName} (ID: {ProductId}), " +
                            "Quantity: {Quantity}, Old Stock: {OldStock}, New Stock: {NewStock}",
                            item.Productname, item.Productid, item.Quantity, oldStock, product.Stock);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Product {ProductId} not found for stock deduction", 
                            item.Productid);
                    }
                }
            }
        }

        await _orderRepository.UpdateAsync(order);
        
        _logger.LogInformation(
            "Order confirmation completed - OrderId: {OrderId}, Status: {Status}",
            orderId, order.Status);
    }

    /// <summary>
    /// Cancel order and restore stock (called by webhook on payment cancellation)
    /// </summary>
    public async Task CancelOrderAsync(int orderId, string reason)
    {
        _logger.LogInformation(
            "Cancelling order {OrderId} - Reason: {Reason}", 
            orderId, reason);

        var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found", orderId);
            return;
        }

        // Update order status
        order.Status = "cancelled";
        order.Updatedat = DateTime.UtcNow;

        // If stock was already deducted, restore it
        if (order.Orderitems != null && order.Orderitems.Any())
        {
            foreach (var item in order.Orderitems)
            {
                if (item.Productid.HasValue)
                {
                    var product = await _productRepository.GetByIdAsync(item.Productid.Value);
                    if (product != null)
                    {
                        var oldStock = product.Stock;
                        product.Stock += item.Quantity;
                        product.Updatedat = DateTime.UtcNow;
                        
                        await _productRepository.UpdateAsync(product);
                        
                        _logger.LogInformation(
                            "Stock restored - Product: {ProductName}, " +
                            "Quantity: {Quantity}, Old Stock: {OldStock}, New Stock: {NewStock}",
                            item.Productname, item.Quantity, oldStock, product.Stock);
                    }
                }
            }
        }

        await _orderRepository.UpdateAsync(order);
        
        _logger.LogInformation("Order cancelled - OrderId: {OrderId}", orderId);
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