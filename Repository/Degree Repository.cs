/*using Ai_LibraryApi.Dto;
using Ai_LibraryApi.Dto.ProfileDto;
using Ai_LibraryApi.Helper;
using Ai_LibraryApi.Interfaces;
using Ai_LibraryApi.Mapping;
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
        private readonly Ai_LibraryApiDbContext _context;
        private readonly ILogger<DegreeRepository> _logger;

        public DegreeRepository(Ai_LibraryApiDbContext context, ILogger<DegreeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
       
        public async Task<PagedResult<DegreeDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.Set<Degree>();
                var totalCount = await query.CountAsync();
                var entities = await query
                    .OrderBy(d => d.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var items = entities.Select(DegreeMapping.ToDto).ToList();

                return new PagedResult<DegreeDto>(items, totalCount, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving degrees");
                throw;
            }
        }
         

         
         public async Task<DegreeDto> GetByIdAsync(Guid id)
        {
            try
            {
                var degree = await _context.Set<Degree>().FindAsync(id);
                if (degree == null) return null;
                return  DegreeMapping.ToDto(degree);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving degree with ID {id}");
                throw;
            }
        }
         
        public async Task<DegreeDto> AddAsync(CreateDegreeDto dto)
        {
            try
            {
                var  degree = DegreeMapping.ToEntity(dto);
                _context.Set<Degree>().Add(degree);
                await _context.SaveChangesAsync();
                return DegreeMapping.ToDto(degree);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding degree");
                throw;
            }
        }
        public async Task<ProfileDto?> UpdateAsync(Guid id, UpdateProfileDto dto)
        {
            try
            {
                var existing = await _context.Set<Profile>().FindAsync(id);
                if (existing == null) return null;

                ProfileMapping.UpdateEntity(existing, dto);
                await _context.SaveChangesAsync();

                return ProfileMapping.ToDto(existing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating profile {id}");
                throw;
            }
        }


        public async Task<ProfileDto?> UpdateAsync(Guid id, UpdateProfileDto dto, Profile entity)
        {
            try
            {
                var existing = await _context.Set<Profile>().FindAsync(id);
                if (existing == null) return null;
                entity.Address = dto.Address ?? existing.Address;
                entity.Country = dto.Country ?? existing.Country;
                entity.Birthdate = dto.Birthdate ?? existing.Birthdate;
                entity.Bio = dto.Bio ?? existing.Bio;
                entity.Photo = dto.Photo ?? existing.Photo;
                entity.UpdatedAt = DateTime.UtcNow;


                ProfileMapping.UpdateEntity(existing, dto);
                await _context.SaveChangesAsync();

                return ProfileMapping.ToDto(existing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating profile {id}");
                throw;
            }
        }
        public async Task<DegreeDto?> UpdateAsync(Guid id,UpdateDegreeDto dto,Degree entity)
        {
            try
            {
                var existingDegree = await _context.Degrees.FindAsync(id);
                if (existingDegree == null) return null;

                entity.DegreeName = dto.DegreeName?? existingDegree.DegreeName;
                entity.University = dto.University?? existingDegree.University;
                entity.Specialization = dto.Specialization?? existingDegree.Specialization;
                entity.UpdatedAt = DateTime.UtcNow;
                DegreeMapping.UpdateEntity(existingDegree, dto);
                await _context.SaveChangesAsync();
                return DegreeMapping.ToDto(existingDegree);
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
}*/