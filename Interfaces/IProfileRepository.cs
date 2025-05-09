using Ai_LibraryApi.Dto.ProfileDto;
using Ai_LibraryApi.Helper;

namespace Ai_LibraryApi.Interfaces
{
    public interface IProfileRepository
    {
        Task<PagedResult<ProfileDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<ProfileDto?> GetByIdAsync(Guid id);
        Task<ProfileDto> AddAsync(CreateProfileDto dto);
        Task<ProfileDto?> UpdateAsync(Guid id, UpdateProfileDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
