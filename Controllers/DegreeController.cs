using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ai_LibraryApi.Dto;
using Microsoft.Extensions.Logging;
using Ai_LibraryApi.Helper;
using Ai_LibraryApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace Ai_LibraryApi.Controllers
{
    [Route("api/[controller]")]
     [ApiController]
    
    public class DegreeController : ControllerBase
    {
        private readonly Ai_LibraryApiDbContext _context;
        private readonly ILogger<DegreeController> _logger;

        public DegreeController(Ai_LibraryApiDbContext context, ILogger<DegreeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Policy = "UserOrAdmin")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<DegreeDto>>>> GetDegrees(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                _logger.LogInformation("Getting degrees with pagination. Page: {PageNumber}, Size: {PageSize}, Search: {SearchTerm}",
                    pageNumber, pageSize, searchTerm ?? "none");

                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var query = _context.Degrees.AsQueryable();

                // Apply search if provided
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(d =>
                        (d.DegreeName != null && d.DegreeName.ToLower().Contains(searchTerm)) ||
                        (d.University != null && d.University.ToLower().Contains(searchTerm)) ||
                        (d.Specialization != null && d.Specialization.ToLower().Contains(searchTerm))
                    );
                }

                var totalCount = await query.CountAsync();
                var degrees = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var degreeDtos = degrees.Select(d => new DegreeDto
                {
                    UniqueID = d.UniqueID,
                    DegreeName = d.DegreeName,
                    University = d.University,
                    Specialization = d.Specialization,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    ProfileId = d.ProfileId
                }).ToList();

                var pagedResult = new PagedResult<DegreeDto>(
                    degreeDtos,
                    totalCount,
                    pageNumber,
                    pageSize
                );

                _logger.LogInformation("Retrieved {Count} degrees out of {TotalCount} total", degreeDtos.Count, totalCount);

                return new ApiResponse<PagedResult<DegreeDto>>(
                    true,
                    "Degrees retrieved successfully",
                    pagedResult
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving degrees");
                return StatusCode(500, new ApiResponse<PagedResult<DegreeDto>>(
                    false,
                    "An error occurred while retrieving degrees",
                    null
                ));
            }
        }

       

        [Authorize(Policy = "UserOrAdmin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<DegreeDto>>> GetDegree(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting degree with ID: {DegreeId}", id);

                var degree = await _context.Degrees.FindAsync(id);

                if (degree == null)
                {
                    _logger.LogWarning("Degree with ID {DegreeId} not found", id);
                    return NotFound(new ApiResponse<DegreeDto>(
                        false,
                        "Degree not found",
                        null
                    ));
                }

                var degreeDto = new DegreeDto
                {
                    UniqueID = degree.UniqueID,
                    DegreeName = degree.DegreeName,
                    University = degree.University,
                    Specialization = degree.Specialization,
                    CreatedAt = degree.CreatedAt,
                    UpdatedAt = degree.UpdatedAt,
                    ProfileId = degree.ProfileId
                };

                _logger.LogInformation("Retrieved degree with ID: {DegreeId}", id);

                return new ApiResponse<DegreeDto>(
                    true,
                    "Degree retrieved successfully",
                    degreeDto
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving degree with ID: {DegreeId}", id);
                return StatusCode(500, new ApiResponse<DegreeDto>(
                    false,
                    "An error occurred while retrieving the degree",
                    null
                ));
            }
        }



        [Authorize(Roles = "Admin")]
        // POST: api/Degree
        [HttpPost]
        public async Task<ActionResult<ApiResponse<DegreeDto>>> CreateDegree(CreateDegreeDto createDegreeDto)
        {
            try
            {
                _logger.LogInformation("Creating new degree for profile ID: {ProfileId}", createDegreeDto.ProfileId);

                // Check if profile exists
                var profileExists = await _context.Profiles.AnyAsync(p => p.ID == createDegreeDto.ProfileId);
                if (!profileExists)
                {
                    _logger.LogWarning("Profile with ID {ProfileId} not found when creating degree", createDegreeDto.ProfileId);
                    return BadRequest(new ApiResponse<DegreeDto>(
                        false,
                        "Profile not found",
                        null
                    ));
                }

                var degree = new Degree
                {
                    DegreeName = createDegreeDto.DegreeName,
                    University = createDegreeDto.University,
                    Specialization = createDegreeDto.Specialization,
                    ProfileId = createDegreeDto.ProfileId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Degrees.Add(degree);
                await _context.SaveChangesAsync();

                var degreeDto = new DegreeDto
                {
                    UniqueID = degree.UniqueID,
                    DegreeName = degree.DegreeName,
                    University = degree.University,
                    Specialization = degree.Specialization,
                    CreatedAt = degree.CreatedAt,
                    UpdatedAt = degree.UpdatedAt,
                    ProfileId = degree.ProfileId
                };

                _logger.LogInformation("Created new degree with ID: {DegreeId} for profile ID: {ProfileId}",
                    degree.UniqueID, degree.ProfileId);

                return CreatedAtAction(
                    nameof(GetDegree),
                    new { id = degree.UniqueID },
                    new ApiResponse<DegreeDto>(
                        true,
                        "Degree created successfully",
                        degreeDto
                    )
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating degree for profile ID: {ProfileId}", createDegreeDto.ProfileId);
                return StatusCode(500, new ApiResponse<DegreeDto>(
                    false,
                    "An error occurred while creating the degree",
                    null
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<DegreeDto>>> UpdateDegree(Guid id, UpdateDegreeDto updateDegreeDto)
        {
            try
            {
                _logger.LogInformation("Updating degree with ID: {DegreeId}", id);

                var degree = await _context.Degrees.FindAsync(id);
                if (degree == null)
                {
                    _logger.LogWarning("Degree with ID {DegreeId} not found during update", id);
                    return NotFound(new ApiResponse<DegreeDto>(
                        false,
                        "Degree not found",
                        null
                    ));
                }

                // Update properties
                degree.DegreeName = updateDegreeDto.DegreeName ?? degree.DegreeName;
                degree.University = updateDegreeDto.University ?? degree.University;
                degree.Specialization = updateDegreeDto.Specialization ?? degree.Specialization;
                degree.UpdatedAt = DateTime.UtcNow;

                _context.Entry(degree).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                var degreeDto = new DegreeDto
                {
                    UniqueID = degree.UniqueID,
                    DegreeName = degree.DegreeName,
                    University = degree.University,
                    Specialization = degree.Specialization,
                    CreatedAt = degree.CreatedAt,
                    UpdatedAt = degree.UpdatedAt,
                    ProfileId = degree.ProfileId
                };

                _logger.LogInformation("Updated degree with ID: {DegreeId}", id);

                return new ApiResponse<DegreeDto>(
                    true,
                    "Degree updated successfully",
                    degreeDto
                );
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!DegreeExists(id))
                {
                    _logger.LogWarning("Degree with ID {DegreeId} not found during concurrency exception", id);
                    return NotFound(new ApiResponse<DegreeDto>(
                        false,
                        "Degree not found",
                        null
                    ));
                }
                else
                {
                    _logger.LogError(ex, "Concurrency error updating degree with ID: {DegreeId}", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating degree with ID: {DegreeId}", id);
                return StatusCode(500, new ApiResponse<DegreeDto>(
                    false,
                    "An error occurred while updating the degree",
                    null
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteDegree(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting degree with ID: {DegreeId}", id);

                var degree = await _context.Degrees.FindAsync(id);
                if (degree == null)
                {
                    _logger.LogWarning("Degree with ID {DegreeId} not found during delete", id);
                    return NotFound(new ApiResponse<bool>(
                        false,
                        "Degree not found",
                        false
                    ));
                }

                _context.Degrees.Remove(degree);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted degree with ID: {DegreeId}", id);

                return new ApiResponse<bool>(
                    true,
                    "Degree deleted successfully",
                    true
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting degree with ID: {DegreeId}", id);
                return StatusCode(500, new ApiResponse<bool>(
                    false,
                    "An error occurred while deleting the degree",
                    false
                ));
            }
        }

        private bool DegreeExists(Guid id)
        {
            return _context.Degrees.Any(e => e.UniqueID == id);
        }
    }
}