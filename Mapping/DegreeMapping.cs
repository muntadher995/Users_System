using Ai_LibraryApi.Dto;
using Ai_LibraryApi.Models;

namespace Ai_LibraryApi.Mapping
{
    public static class DegreeMapping
    {
        public static DegreeDto ToDto(Degree degree)
        {
            return new DegreeDto
            {
                UniqueID = degree.UniqueID,
                DegreeName = degree.DegreeName,
                University = degree.University,
                Specialization = degree.Specialization,
                CreatedAt = degree.CreatedAt,
                UpdatedAt = degree.UpdatedAt,
                ProfileId = degree.ProfileId
            };
        }

        public static Degree ToEntity(CreateDegreeDto dto)
        {
            return new Degree
            {
                UniqueID = Guid.NewGuid(),
                DegreeName = dto.DegreeName,
                University = dto.University,
                Specialization = dto.Specialization,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ProfileId = dto.ProfileId
            };
        }

        public static void UpdateEntity(Degree entity, UpdateDegreeDto dto)
        {
            entity.DegreeName = dto.DegreeName;
            entity.University = dto.University;
            entity.Specialization = dto.Specialization;
            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}