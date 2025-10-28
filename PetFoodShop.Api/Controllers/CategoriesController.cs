using Microsoft.AspNetCore.Mvc;
using PetFoodShop.Api.Dtos;
using PetFoodShop.Api.Services.Interfaces;

namespace PetFoodShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound(new { message = "Category not found" });
        }

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto createDto)
    {
        var category = await _categoryService.CreateCategoryAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] UpdateCategoryDto updateDto)
    {
        var category = await _categoryService.UpdateCategoryAsync(id, updateDto);
        if (category == null)
        {
            return NotFound(new { message = "Category not found" });
        }

        return Ok(category);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        if (!result)
        {
            return NotFound(new { message = "Category not found" });
        }

        return NoContent();
    }
}
