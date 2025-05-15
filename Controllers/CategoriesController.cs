using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ai_LibraryApi.DTOs;
using Ai_LibraryApi.Models;
 
using Ai_LibraryApi.Helper;
using Microsoft.AspNetCore.Authorization;

namespace Ai_LibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly Ai_LibraryApiDbContext _context;
      
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(Ai_LibraryApiDbContext context, ILogger<CategoriesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Policy = "UserOrAdmin")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<CategoryDto>>>> GetCategories(
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10,
           [FromQuery] string? searchTerm = null)
        {
            try
            {
                
                 

                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var query = _context.Categories.AsQueryable();

               
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    _logger.LogInformation("Applying search filter: {SearchTerm}", searchTerm);
                    query = query.Where(c =>
                        !string.IsNullOrEmpty(c.Title) &&
                        EF.Functions.Like(c.Title!, $"%{searchTerm}%"));
                }


                var totalCount = await query.CountAsync();
                var categories = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var categoryDtos = categories.Select(c => new CategoryDto
                {
                    UniqueID = c.UniqueID,
                    Title = c.Title,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList();

                var pagedResult = new PagedResult<CategoryDto>(
                    categoryDtos,
                    totalCount,
                    pageNumber,
                    pageSize
                );

                _logger.LogInformation("Retrieved {Count} categories out of {TotalCount} total", categoryDtos.Count, totalCount);

                return new ApiResponse<PagedResult<CategoryDto>>(
                    true,
                    "Categories retrieved successfully",
                    pagedResult
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return StatusCode(500, new ApiResponse<PagedResult<CategoryDto>>(
                    false,
                    "An error occurred while retrieving categories",
                    null
                ));
            }
        }


        [Authorize(Policy = "UserOrAdmin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategory(Guid id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                {
                    return NotFound(new ApiResponse<CategoryDto>(
                        false,
                        "Category not found",
                        null
                    ));
                }

                var categoryDto = new CategoryDto
                {
                    UniqueID = category.UniqueID,
                    Title = category.Title,
                    Status = category.Status,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt
                };

                return Ok(new ApiResponse<CategoryDto>(
                    true,
                    "Category retrieved successfully",
                    categoryDto
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CategoryDto>(
                    false,
                    $"Error retrieving category: {ex.Message}",
                    null
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            try
            {
                var category = new Category
                {
                    UniqueID = Guid.NewGuid(),
                    Title = createCategoryDto.Title,
                    Status = createCategoryDto.Status,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                var categoryDto = new CategoryDto
                {
                    UniqueID = category.UniqueID,
                    Title = category.Title,
                    Status = category.Status,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt
                };

                return CreatedAtAction(nameof(GetCategory), new { id = category.UniqueID },
                    new ApiResponse<CategoryDto>(
                        true,
                        "Category created successfully",
                        categoryDto
                    ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CategoryDto>(
                    false,
                    $"Error creating category: {ex.Message}",
                    null
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(Guid id, UpdateCategoryDto updateCategoryDto)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound(new ApiResponse<CategoryDto>(
                        false,
                        "Category not found",
                        null
                    ));
                }

                category.Title = updateCategoryDto.Title;

                if (updateCategoryDto.Status.HasValue)
                {
                    category.Status = updateCategoryDto.Status.Value;
                }

                 
                category.UpdatedAt = DateTime.UtcNow;

                _context.Entry(category).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(id))
                    {
                        return NotFound(new ApiResponse<CategoryDto>(
                            false,
                            "Category not found",
                            null
                        ));
                    }
                    else
                    {
                        throw;
                    }
                }

                var categoryDto = new CategoryDto
                {
                    UniqueID = category.UniqueID,
                    Title = category.Title,
                    Status = category.Status,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt
                };

                return Ok(new ApiResponse<CategoryDto>(
                    true,
                    "Category updated successfully",
                    categoryDto
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CategoryDto>(
                    false,
                    $"Error updating category: {ex.Message}",
                    null
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(Guid id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound(new ApiResponse<bool>(
                        false,
                        "Category not found",
                        false
                    ));
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<bool>(
                    true,
                    "Category deleted successfully",
                    true
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>(
                    false,
                    $"Error deleting category: {ex.Message}",
                    false
                ));
            }
        }

        private bool CategoryExists(Guid id)
        {
            return _context.Categories.Any(e => e.UniqueID == id);
        }
    }
}