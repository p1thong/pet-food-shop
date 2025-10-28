using Microsoft.AspNetCore.Mvc;
using PetFoodShop.Api.Dtos;
using PetFoodShop.Api.Services.Interfaces;

namespace PetFoodShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(
        [FromQuery] bool? activeOnly = null, 
        [FromQuery] int? categoryId = null)
    {
        IEnumerable<ProductDto> products;

        if (categoryId.HasValue)
        {
            products = await _productService.GetProductsByCategoryAsync(categoryId.Value);
        }
        else if (activeOnly == true)
        {
            products = await _productService.GetActiveProductsAsync();
        }
        else
        {
            products = await _productService.GetAllProductsAsync();
        }

        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound(new { message = "Product not found" });
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto createDto)
    {
        var product = await _productService.CreateProductAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto updateDto)
    {
        var product = await _productService.UpdateProductAsync(id, updateDto);
        if (product == null)
        {
            return NotFound(new { message = "Product not found" });
        }

        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _productService.DeleteProductAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Product not found" });
        }

        return NoContent();
    }

    [HttpPatch("{id}/soft-delete")]
    public async Task<ActionResult> SoftDelete(int id)
    {
        var result = await _productService.SoftDeleteProductAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Product not found" });
        }

        return NoContent();
    }
}
