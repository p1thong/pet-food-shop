using PetFoodShop.Api.Dtos;
using PetFoodShop.Api.Models;
using PetFoodShop.Api.Repositories.Interfaces;
using PetFoodShop.Api.Services.Interfaces;

namespace PetFoodShop.Api.Services.Implements;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category == null ? null : MapToDto(category);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto)
    {
        var category = new Category
        {
            Name = createDto.Name,
            Description = createDto.Description,
        };

        var createdCategory = await _categoryRepository.AddAsync(category);
        return MapToDto(createdCategory);
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return null;
        }

        category.Name = updateDto.Name;
        category.Description = updateDto.Description;

        await _categoryRepository.UpdateAsync(category);
        return MapToDto(category);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var exists = await _categoryRepository.ExistsAsync(id);
        if (!exists)
        {
            return false;
        }

        await _categoryRepository.DeleteAsync(id);
        return true;
    }

    private CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
        };
    }
}
