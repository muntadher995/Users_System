using Ai_LibraryApi.Dto;
using Ai_LibraryApi.Helper;
using Ai_LibraryApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ai_LibraryApi.Interfaces
{
    public interface IDegreeRepository
    {
        Task<PagedResult<DegreeDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<DegreeDto> GetByIdAsync(Guid id);
        Task<DegreeDto> AddAsync(CreateDegreeDto dto);
        Task<DegreeDto?> UpdateAsync(Guid id, UpdateDegreeDto dto, Degree entity);
        Task<bool> DeleteAsync(Guid id);
    }
}