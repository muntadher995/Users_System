using Ai_LibraryApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ai_LibraryApi.Interfaces
{
    public interface IDegreeRepository
    {
        Task<IEnumerable<Degree>> GetAllAsync(int pageNumber, int pageSize);
        Task<Degree?> GetByIdAsync(Guid id);
        Task<Degree> AddAsync(Degree degree);
        Task<Degree?> UpdateAsync(Guid id, Degree degree);
        Task<bool> DeleteAsync(Guid id);
    }
}