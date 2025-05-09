using Ai_LibraryApi.Dto;
using Ai_LibraryApi.Interfaces;
using Ai_LibraryApi.Models;
using Ai_LibraryApi.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ai_LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DegreeController : ControllerBase
    {
        private readonly IDegreeRepository _repository;
        private readonly ILogger<DegreeController> _logger;

        public DegreeController(IDegreeRepository repository, ILogger<DegreeController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var degrees = await _repository.GetAllAsync(pageNumber, pageSize);
                var degreeDtos = degrees.Select(DegreeMapping.ToDto);
                return Ok(new ApiResponse<IEnumerable<DegreeDto>>(true, "Degrees retrieved successfully", degreeDtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving degrees");
                return StatusCode(500, new ApiResponse<string>(false, $"Error: {ex.Message}", null));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var degree = await _repository.GetByIdAsync(id);
                if (degree == null) return NotFound(new ApiResponse<string>(false, "Degree not found", null));
                return Ok(new ApiResponse<DegreeDto>(true, "Degree retrieved successfully", DegreeMapping.ToDto(degree)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving degree with ID {id}");
                return StatusCode(500, new ApiResponse<string>(false, $"Error: {ex.Message}", null));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDegreeDto dto)
        {
            try
            {
                var degree = DegreeMapping.ToEntity(dto);
                var createdDegree = await _repository.AddAsync(degree);
                return CreatedAtAction(nameof(GetById), new { id = createdDegree.UniqueID }, new ApiResponse<DegreeDto>(true, "Degree created successfully", DegreeMapping.ToDto(createdDegree)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating degree");
                return StatusCode(500, new ApiResponse<string>(false, $"Error: {ex.Message}", null));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDegreeDto dto)
        {
            try
            {
                var degree = DegreeMapping.ToEntity(dto);
                var updatedDegree = await _repository.UpdateAsync(id, degree);
                if (updatedDegree == null) return NotFound(new ApiResponse<string>(false, "Degree not found", null));
                return Ok(new ApiResponse<DegreeDto>(true, "Degree updated successfully", DegreeMapping.ToDto(updatedDegree)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating degree with ID {id}");
                return StatusCode(500, new ApiResponse<string>(false, $"Error: {ex.Message}", null));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted) return NotFound(new ApiResponse<string>(false, "Degree not found", null));
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting degree with ID {id}");
                return StatusCode(500, new ApiResponse<string>(false, $"Error: {ex.Message}", null));
            }
        }
    }
}