using Ai_LibraryApi.Interfaces;
using Ai_LibraryApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ai_LibraryApi.Repository
{
    public class DegreeRepository : IDegreeRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DegreeRepository> _logger;

        public DegreeRepository(AppDbContext context, ILogger<DegreeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Degree>> GetAllAsync(int pageNumber, int pageSize)
        {
            try
            {
                return await _context.Degrees
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving degrees");
                throw;
            }
        }

        public async Task<Degree?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.Degrees.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving degree with ID {id}");
                throw;
            }
        }

        public async Task<Degree> AddAsync(Degree degree)
        {
            try
            {
                _context.Degrees.Add(degree);
                await _context.SaveChangesAsync();
                return degree;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding degree");
                throw;
            }
        }

        public async Task<Degree?> UpdateAsync(Guid id, Degree degree)
        {
            try
            {
                var existingDegree = await _context.Degrees.FindAsync(id);
                if (existingDegree == null) return null;

                existingDegree.DegreeName = degree.DegreeName;
                existingDegree.University = degree.University;
                existingDegree.Specialization = degree.Specialization;
                existingDegree.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return existingDegree;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating degree with ID {id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var degree = await _context.Degrees.FindAsync(id);
                if (degree == null) return false;

                _context.Degrees.Remove(degree);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting degree with ID {id}");
                throw;
            }
        }
    }
}