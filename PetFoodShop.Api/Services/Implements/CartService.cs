using PetFoodShop.Api.Dtos;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Interfaces;
using PetFoodShop.Api.Services.Interfaces;

namespace PetFoodShop.Api.Services.Implements;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<CartDto?> GetCartByUserIdAsync(int userId)
    {
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);
        return cart == null ? null : MapToDto(cart);
    }

    public async Task<CartDto> AddToCartAsync(AddToCartDto addToCartDto)
    {
        var product = await _productRepository.GetByIdAsync(addToCartDto.Productid);
        if (product == null)
            throw new InvalidOperationException("Product not found");

        var cart = await _cartRepository.GetCartByUserIdAsync(addToCartDto.Userid);
        
        if (cart == null)
        {
            // Create new cart
            cart = new Cart
            {
                Userid = addToCartDto.Userid,
                Createdat = DateTime.Now,
                Updatedat = DateTime.Now,
            };
            cart = await _cartRepository.AddAsync(cart);
        }

        // Check if product already in cart
        var existingItem = cart.Cartitems?.FirstOrDefault(
            ci => ci.Productid == addToCartDto.Productid);
        
        if (existingItem != null)
        {
            existingItem.Quantity += addToCartDto.Quantity;
            await _cartRepository.UpdateCartItemAsync(existingItem);
        }
        else
        {
            var cartItem = new Cartitem
            {
                Cartid = cart.Id,
                Productid = addToCartDto.Productid,
                Quantity = addToCartDto.Quantity,
                Pricesnapshot = product.Price,
                Addedat = DateTime.Now,
            };
            await _cartRepository.AddCartItemAsync(cartItem);
        }

        cart = await _cartRepository.GetCartWithItemsAsync(cart.Id);
        return MapToDto(cart!);
    }

    public async Task<CartDto?> UpdateCartItemAsync(int cartItemId, UpdateCartItemDto updateDto)
    {
        var cartItem = await _cartRepository.GetCartItemAsync(cartItemId);
        if (cartItem == null)
        {
            return null;
        }

        cartItem.Quantity = updateDto.Quantity;
        await _cartRepository.UpdateCartItemAsync(cartItem);

        var cart = await _cartRepository.GetCartWithItemsAsync(cartItem.Cartid!.Value);
        return cart == null ? null : MapToDto(cart);
    }

    public async Task<bool> RemoveFromCartAsync(int cartItemId)
    {
        var cartItem = await _cartRepository.GetCartItemAsync(cartItemId);
        if (cartItem == null)
        {
            return false;
        }

        await _cartRepository.DeleteCartItemAsync(cartItemId);
        return true;
    }

    public async Task<bool> ClearCartAsync(int userId)
    {
        var cart = await _cartRepository.GetCartByUserIdAsync(userId);
        if (cart == null)
        {
            return false;
        }

        foreach (var item in cart.Cartitems)
        {
            await _cartRepository.DeleteCartItemAsync(item.Id);
        }
        return true;
    }

    private CartDto MapToDto(Cart cart)
    {
        var cartDto = new CartDto
        {
            Id = cart.Id,
            Userid = cart.Userid,
            Createdat = cart.Createdat,
            Updatedat = cart.Updatedat,
            CartItems = cart.Cartitems?.Select(ci => new CartItemDto
            {
                Id = ci.Id,
                Productid = ci.Productid,
                ProductName = ci.Product?.Name,
                ProductSku = ci.Product?.Sku,
                ProductImage = ci.Product?.Imageurl,
                Quantity = ci.Quantity,
                Pricesnapshot = ci.Pricesnapshot,
                Addedat = ci.Addedat,
            }).ToList(),
            TotalAmount = cart.Cartitems?.Sum(ci => ci.Quantity * ci.Pricesnapshot) ?? 0,
        };

        return cartDto;
    }
}

