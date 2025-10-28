using PetFoodShop.Api.Dtos;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Interfaces;
using PetFoodShop.Api.Services.Interfaces;

namespace PetFoodShop.Api.Services.Implements;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetActiveProductsAsync()
    {
        var products = await _productRepository.GetActiveProductsAsync();
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetProductWithCategoryAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
    {
        var product = new Product
        {
            Sku = createDto.Sku,
            Name = createDto.Name,
            Description = createDto.Description,
            Price = createDto.Price,
            Stock = createDto.Stock ?? 0,
            Categoryid = createDto.Categoryid,
            Imageurl = createDto.Imageurl,
            Isdeleted = false,
            Createdat = DateTime.Now,
            Updatedat = DateTime.Now,
        };

        var createdProduct = await _productRepository.AddAsync(product);
        return MapToDto(createdProduct);
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return null;
        }

        if (updateDto.Sku != null) product.Sku = updateDto.Sku;
        if (updateDto.Name != null) product.Name = updateDto.Name;
        if (updateDto.Description != null) product.Description = updateDto.Description;
        if (updateDto.Price.HasValue) product.Price = updateDto.Price.Value;
        if (updateDto.Stock.HasValue) product.Stock = updateDto.Stock;
        if (updateDto.Categoryid.HasValue) product.Categoryid = updateDto.Categoryid;
        if (updateDto.Imageurl != null) product.Imageurl = updateDto.Imageurl;
        if (updateDto.Isdeleted.HasValue) product.Isdeleted = updateDto.Isdeleted.Value;
        
        product.Updatedat = DateTime.Now;

        await _productRepository.UpdateAsync(product);
        return MapToDto(product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var exists = await _productRepository.ExistsAsync(id);
        if (!exists)
        {
            return false;
        }

        await _productRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> SoftDeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return false;
        }

        product.Isdeleted = true;
        product.Updatedat = DateTime.Now;
        await _productRepository.UpdateAsync(product);
        return true;
    }

    private ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Sku = product.Sku,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Categoryid = product.Categoryid,
            CategoryName = product.Category?.Name,
            Imageurl = product.Imageurl,
            Isdeleted = product.Isdeleted,
            Createdat = product.Createdat,
            Updatedat = product.Updatedat,
        };
    }
}
