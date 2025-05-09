



using Ai_LibraryApi.Dto;
using Ai_LibraryApi.Dto.ProfileDto;
using Ai_LibraryApi.Helper;
using Ai_LibraryApi.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ai_LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileRepository _repository;

        public ProfileController(IProfileRepository repository)
        {
            _repository = repository;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagedResult = await _repository.GetAllAsync(pageNumber, pageSize);
                return Ok(new ApiResponse<IEnumerable<ProfileDto>>(true, "Profiles retrieved successfully", pagedResult.Items));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(false, $"Error: {ex.Message}", null));
            }
        }
       

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var profile = await _repository.GetByIdAsync(id);
                if (profile == null) return NotFound(new ApiResponse<string>(false, "Profile not found", null));
                return Ok(new ApiResponse<ProfileDto>(true, "Profile retrieved successfully", profile));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(false, $"Error: {ex.Message}", null));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProfileDto dto)
        {
            try
            {
                var profileDto = await _repository.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = profileDto.UniqueID }, new ApiResponse<ProfileDto>(true, "Profile created successfully", profileDto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(false, $"Error: {ex.Message}", null));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProfileDto dto)
        {
            try
            {
                var updatedProfile = await _repository.UpdateAsync(id, dto);
                if (updatedProfile == null) return NotFound(new ApiResponse<string>(false, "Profile not found", null));
                return Ok(new ApiResponse<ProfileDto>(true, "Profile updated successfully", updatedProfile));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(false, $"Error: {ex.Message}", null));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted) return NotFound(new ApiResponse<string>(false, "Profile not found", null));
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(false, $"Error: {ex.Message}", null));
            }
        }
    }
}












/*using Ai_LibraryApi.Dto.ProfileDto;
using Ai_LibraryApi.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;



namespace Ai_LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileRepository _repository;

        public ProfileController(IProfileRepository repository)
        {
            _repository = repository;
        }

      

       
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var profiles = await _repository.GetAllAsync(pageNumber, pageSize);
                return Ok(profiles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
       




        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var profile = await _repository.GetByIdAsync(id);
                if (profile == null) return NotFound();
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProfileDto dto)
        {
            try
            {
                var profileDto = await _repository.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = profileDto.UniqueID }, profileDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProfileDto dto)
        {
            try
            {
                var updatedProfile = await _repository.UpdateAsync(id, dto);
                if (updatedProfile == null) return NotFound();
                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
*/