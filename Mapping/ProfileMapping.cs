using Ai_LibraryApi.Dto.ProfileDto;
using Ai_LibraryApi.Models;


namespace Ai_LibraryApi.Mapping
{
   
        public static class ProfileMapping
        {
            public static ProfileDto ToDto(Profile profile)
            {
                return new ProfileDto
                {
                    UniqueID = profile.ID,
                    Address = profile.Address,
                    Country = profile.Country,
                    Birthdate = profile.Birthdate,
                    Bio = profile.Bio,
                    Photo = profile.Photo,
                    CreatedAt = profile.CreatedAt,
                    UpdatedAt = profile.UpdatedAt,
                    UserId = profile.UserId
                };
            }

            public static Profile ToEntity(CreateProfileDto dto)
            {
                return new Profile
                {
                    ID = Guid.NewGuid(),
                    Address = dto.Address,
                    Country = dto.Country,
                    Birthdate = dto.Birthdate,
                    Bio = dto.Bio,
                    Photo = dto.Photo,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = dto.UserId
                };
            }

            public static void UpdateEntity(Profile entity, UpdateProfileDto dto)
            {
                entity.Address = dto.Address;
                entity.Country = dto.Country;
                entity.Birthdate = dto.Birthdate;
                entity.Bio = dto.Bio;
                entity.Photo = dto.Photo;
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }

