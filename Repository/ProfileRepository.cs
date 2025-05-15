/*
using Ai_LibraryApi.Models;
using Microsoft.Extensions.Logging;
using Ai_LibraryApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using Ai_LibraryApi.Dto.ProfileDto;
using Ai_LibraryApi.Helper;
using Ai_LibraryApi.Mapping;


namespace Ai_LibraryApi.Repository
{

   
        public class ProfileRepository : IProfileRepository
        {
            private readonly Ai_LibraryApiDbContext _context;
            private readonly ILogger<ProfileRepository> _logger;

            public ProfileRepository(Ai_LibraryApiDbContext context, ILogger<ProfileRepository> logger)
            {
                _context = context;
                _logger = logger;
            }

        public async Task<PagedResult<ProfileDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            try
            {
                var query = _context.Set<Profile>();
                var totalCount = await query.CountAsync();
                var entities = await query
                    .OrderBy(p => p.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var items = entities.Select(ProfileMapping.ToDto).ToList();

                return new PagedResult<ProfileDto>(items, totalCount, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged profiles");
                throw;
            }
        }

        public async Task<ProfileDto?> GetByIdAsync(Guid id)
            {
                try
                {
                    var p = await _context.Set<Profile>().FindAsync(id);
                    if (p == null) return null;
                    return ProfileMapping.ToDto(p);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error getting profile by id {id}");
                    throw;
                }
            }

            public async Task<ProfileDto> AddAsync(CreateProfileDto dto)
            {
                try
                {
                    var profile = ProfileMapping.ToEntity(dto);
                    _context.Set<Profile>().Add(profile);
                    await _context.SaveChangesAsync();
                    return ProfileMapping.ToDto(profile);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding profile");
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

            public async Task<bool> DeleteAsync(Guid id)
            {
                try
                {
                    var profile = await _context.Set<Profile>().FindAsync(id);
                    if (profile == null) return false;

                    _context.Set<Profile>().Remove(profile);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error deleting profile {id}");
                    throw;
                }
            }
        }
    }
*/