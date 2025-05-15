
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ai_LibraryApi.Models;
using Ai_LibraryApi.Dto.ProfileDto;
using Ai_LibraryApi.Helper;
using Ai_LibraryApi.Services;
using Ai_LibraryApi.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;

namespace Ai_LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
 
    public class ProfileController : ControllerBase
    {
        private readonly Ai_LibraryApiDbContext _context;
        private readonly ILogger<ProfileController> _logger;
        private readonly FileService _fileService;

        public ProfileController(Ai_LibraryApiDbContext context, ILogger<ProfileController> logger, FileService fileService)
        {
            _context = context;
            _logger = logger;
            _fileService = fileService;
        }


        [Authorize(Policy = "UserOrAdmin")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ProfileDto>>>> GetProfiles(string? search, string? approvalStatus, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.Profiles.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var loweredSearch = search.Trim().ToLower();
                    query = query.Where(p =>
                        (p.Address != null && p.Address.Trim().ToLower().Contains(loweredSearch)) ||
                        (p.Country != null && p.Country.Trim().ToLower().Contains(loweredSearch)) ||
                        (p.Bio != null && p.Bio.Trim().ToLower().Contains(loweredSearch))
                    );
                }

                // فلترة حسب حالة الموافقة إذا تم توفيرها
                if (!string.IsNullOrWhiteSpace(approvalStatus))
                {
                    query = query.Where(p => p.ApprovalStatus == approvalStatus);
                }

                var totalCount = await query.CountAsync();

                var profiles = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProfileDto
                    {
                        UniqueID = p.ID,
                        Address = p.Address,
                        Country = p.Country,
                        Birthdate = p.Birthdate,
                        Bio = p.Bio,
                        // Dynamically construct full file paths for Cv_File and Photo
                        Cv_File = string.IsNullOrEmpty(p.Cv_File) ? null : $"{Request.Scheme}://{Request.Host}/uploads/profile/{p.Cv_File}",
                        Photo = string.IsNullOrEmpty(p.Photo) ? null : $"{Request.Scheme}://{Request.Host}/uploads/profile/{p.Photo}",
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt,
                        UserId = p.UserId,
                        ApprovalStatus = p.ApprovalStatus,
                        RejectionReason = p.RejectionReason
                    })
                    .ToListAsync();

                var result = new PagedResult<ProfileDto>(profiles, totalCount, pageNumber, pageSize);
                return Ok(new ApiResponse<PagedResult<ProfileDto>>(true, "تم استرجاع الملفات الشخصية بنجاح", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "خطأ في استرجاع الملفات الشخصية");
                return StatusCode(500, new ApiResponse<PagedResult<ProfileDto>>(false, "خطأ في الخادم الداخلي", null));
            }
        }



        [Authorize(Policy = "UserOrAdmin")]
        [HttpGet("profile/{id}")]
        public async Task<ActionResult<ApiResponse<ProfileDto>>> GetProfile(Guid id)
        {
            try
            { 
                var profile = await _context.Profiles.FindAsync(id);
                if (profile == null)
                    return NotFound(new ApiResponse<ProfileDto>(false, "الملف الشخصي غير موجود", null));

                var dto = new ProfileDto
                {
                    UniqueID = profile.ID,
                    Address = profile.Address,
                    Country = profile.Country,
                    Birthdate = profile.Birthdate,
                    Bio = profile.Bio,
                    // Full URL for Cv_File and Photo
                    Cv_File = string.IsNullOrEmpty(profile.Cv_File) ? null : $"{Request.Scheme}://{Request.Host}/uploads/profile/{profile.Cv_File}",
                    Photo = string.IsNullOrEmpty(profile.Photo) ? null : $"{Request.Scheme}://{Request.Host}/uploads/profile/{profile.Photo}",
                    CreatedAt = profile.CreatedAt,
                    UpdatedAt = profile.UpdatedAt,
                    UserId = profile.UserId,
                    ApprovalStatus = profile.ApprovalStatus,
                    RejectionReason = profile.RejectionReason
                };

                return Ok(new ApiResponse<ProfileDto>(true, "تم استرجاع الملف الشخصي بنجاح", dto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"خطأ في استرجاع الملف الشخصي {id}");
                return StatusCode(500, new ApiResponse<ProfileDto>(false, "خطأ في الخادم الداخلي", null));
            }
        }


        [Authorize(Policy = "UserOrAdmin")]
        [HttpPost]
        public async Task<IActionResult> CreateProfile([FromForm] CreateProfileDto dto)
        {
            try
            {
                string? photoFileName = null;
                string? cvFileName = null;

                if (dto.PhotoFile != null)
                {
                    photoFileName = await _fileService.SaveImageAsync(dto.PhotoFile, "profile");
                }

                if (dto.Cv_File != null)
                {
                    cvFileName = await _fileService.SaveFileAsync(dto.Cv_File, "profile");
                }

                var profile = new Profile
                {
                    ID = Guid.NewGuid(),
                    Address = dto.Address,
                    Country = dto.Country,
                    Bio = dto.Bio,
                    Birthdate = dto.Birthdate,
                    UserId = dto.UserId,
                    Photo = photoFileName,
                    Cv_File = cvFileName ?? "",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ApprovalStatus = "Pending" // تعيين الحالة الافتراضية إلى "قيد الانتظار"
                };
/*
                var userid = _context.Notifications.Select(n => n.UserId).FirstOrDefault();
                if (dto.UserId != userid)
                {
                    return BadRequest(new ApiResponse<NotificationDto>(
                        false,
                        "User ID does not match",
                        null
                    ));
                }
                
                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();

                // إنشاء إشعار للمستخدم بأن ملفه الشخصي قيد المراجعة
                var notification = new Notifications
                {
                    ID = Guid.NewGuid(),
                    Title = "تم استلام ملفك الشخصي",
                    Message = "تم استلام ملفك الشخصي وهو قيد المراجعة من قبل المسؤول.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    UserId = dto.UserId
                };
                
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();*/

                // تحويل النموذج إلى DTO للاستجابة
                var profileDto = new ProfileDto
                {
                    UniqueID = profile.ID,
                    Address = profile.Address,
                    Country = profile.Country,
                    Birthdate = profile.Birthdate,
                    Bio = profile.Bio,
                    Photo = profile.Photo,
                    Cv_File = profile.Cv_File,
                    CreatedAt = profile.CreatedAt,
                    UpdatedAt = profile.UpdatedAt,
                    UserId = profile.UserId,
                    ApprovalStatus = profile.ApprovalStatus
                };

                return Ok(new ApiResponse<ProfileDto>(true, "تم إنشاء الملف الشخصي بنجاح وهو قيد المراجعة", profileDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنشاء الملف الشخصي");
                return StatusCode(500, new ApiResponse<ProfileDto>(false, "خطأ في الخادم الداخلي", null));
            }
        }

        [Authorize(Roles = "Admin")]

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ProfileDto>>> UpdateProfile(Guid id, [FromForm] UpdateProfileDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<ProfileDto>(false, "بيانات غير صالحة", null));

            try
            {
                var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.ID == id);
                if (profile == null)
                    return NotFound(new ApiResponse<ProfileDto>(false, "الملف الشخصي غير موجود", null));

                profile.Address = dto.Address ?? profile.Address;
                profile.Country = dto.Country ?? profile.Country;
                profile.Birthdate = dto.Birthdate ?? profile.Birthdate;
                profile.Bio = dto.Bio ?? profile.Bio;
                profile.UpdatedAt = DateTime.UtcNow;
                profile.ApprovalStatus = "Pending"; // إعادة تعيين حالة الموافقة إلى "قيد الانتظار" بعد التحديث

                // Handle updating the Photo if a new one is provided
                if (dto.PhotoFile != null && dto.PhotoFile.Length > 0)
                {
                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(profile.Photo))
                    {
                        var oldPhotoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile", profile.Photo);
                        if (System.IO.File.Exists(oldPhotoPath))
                            System.IO.File.Delete(oldPhotoPath);
                    }

                    // Create the directory if it doesn't exist
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    // Save the new photo with a unique file name
                    var newPhotoFileName = Guid.NewGuid() + Path.GetExtension(dto.PhotoFile.FileName);
                    var newPhotoFilePath = Path.Combine(uploadsFolder, newPhotoFileName);

                    using (var stream = new FileStream(newPhotoFilePath, FileMode.Create))
                    {
                        await dto.PhotoFile.CopyToAsync(stream);
                    }

                    // Update the profile's Photo field with the new file name
                    profile.Photo = newPhotoFileName;
                }

                // Handle updating the CV file if a new one is provided
                if (dto.Cv_File != null && dto.Cv_File.Length > 0)
                {
                    // Delete the old CV file if it exists
                    if (!string.IsNullOrEmpty(profile.Cv_File))
                    {
                        var oldCvFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile", profile.Cv_File);
                        if (System.IO.File.Exists(oldCvFilePath))
                            System.IO.File.Delete(oldCvFilePath);
                    }

                    // Create the directory if it doesn't exist
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    // Save the new CV file with a unique file name
                    var newCvFileName = Guid.NewGuid() + Path.GetExtension(dto.Cv_File.FileName);
                    var newCvFilePath = Path.Combine(uploadsFolder, newCvFileName);

                    using (var stream = new FileStream(newCvFilePath, FileMode.Create))
                    {
                        await dto.Cv_File.CopyToAsync(stream);
                    }

                    // Update the profile's Cv_File field with the new file name
                    profile.Cv_File = newCvFileName;
                }

              /*  // إنشاء إشعار للمستخدم بأن ملفه الشخصي قيد المراجعة بعد التحديث
                var notification = new Notifications
                {
                    ID = Guid.NewGuid(),
                    Title = "تم تحديث ملفك الشخصي",
                    Message = "تم تحديث ملفك الشخصي وهو قيد المراجعة من قبل المسؤول.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    UserId = profile.UserId
                };
                
                _context.Notifications.Add(notification);
*/
                // Update profile in the database
                _context.Profiles.Update(profile);
                await _context.SaveChangesAsync();

                var result = new ProfileDto
                {
                    UniqueID = profile.ID,
                    Address = profile.Address,
                    Country = profile.Country,
                    Birthdate = profile.Birthdate,
                    Bio = profile.Bio,
                    Photo = profile.Photo,
                    Cv_File = profile.Cv_File,
                    CreatedAt = profile.CreatedAt,
                    UpdatedAt = profile.UpdatedAt,
                    UserId = profile.UserId,
                    ApprovalStatus = profile.ApprovalStatus
                };

                return Ok(new ApiResponse<ProfileDto>(true, "تم تحديث الملف الشخصي بنجاح وهو قيد المراجعة", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"خطأ في تحديث الملف الشخصي {id}");
                return StatusCode(500, new ApiResponse<ProfileDto>(false, "خطأ في الخادم الداخلي", null));
            }
        }

[Authorize(Roles = "Admin")]
[HttpPut("approve/{id}")]
public async Task<ActionResult<ApiResponse<ProfileDto>>> ApproveProfile(Guid id, [FromBody] ProfileApprovalDto approvalDto)
{
    try
    {
        var profile = await _context.Profiles.FindAsync(id);
        if (profile == null)
            return NotFound(new ApiResponse<ProfileDto>(false, "الملف الشخصي غير موجود", null));

        // التحقق من صحة حالة الموافقة
        if (approvalDto.ApprovalStatus != "Approved" && approvalDto.ApprovalStatus != "Rejected")
            return BadRequest(new ApiResponse<ProfileDto>(false, "حالة الموافقة غير صالحة. يجب أن تكون 'Approved' أو 'Rejected'", null));

        // إذا تم رفض الملف الشخصي، يجب توفير سبب الرفض
        if (approvalDto.ApprovalStatus == "Rejected" && string.IsNullOrWhiteSpace(approvalDto.RejectionReason))
            return BadRequest(new ApiResponse<ProfileDto>(false, "يجب توفير سبب الرفض عند رفض الملف الشخصي", null));

        // تحديث حالة الموافقة وسبب الرفض (إذا وجد)
        profile.ApprovalStatus = approvalDto.ApprovalStatus;
        profile.RejectionReason = approvalDto.RejectionReason;
        profile.UpdatedAt = DateTime.UtcNow;

        _context.Profiles.Update(profile);

      /*  // إنشاء إشعار للمستخدم بناءً على حالة الموافقة
        var notificationTitle = approvalDto.ApprovalStatus == "Approved" ? "تمت الموافقة على ملفك الشخصي" : "تم رفض ملفك الشخصي";
        var notificationMessage = approvalDto.ApprovalStatus == "Approved" 
            ? "تمت الموافقة على ملفك الشخصي ونشره." 
            : $"تم رفض ملفك الشخصي للسبب التالي: {approvalDto.RejectionReason}";

        var notification = new Notifications
        {
            ID = Guid.NewGuid(),
            Title = notificationTitle,
            Message = notificationMessage,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            UserId = profile.UserId
        };
        
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();*/

        var result = new ProfileDto
        {
            UniqueID = profile.ID,
            Address = profile.Address,
            Country = profile.Country,
            Birthdate = profile.Birthdate,
            Bio = profile.Bio,
            Photo = profile.Photo,
            Cv_File = profile.Cv_File,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt,
            UserId = profile.UserId,
            ApprovalStatus = profile.ApprovalStatus,
            RejectionReason = profile.RejectionReason
        };

        return Ok(new ApiResponse<ProfileDto>(true, $"تم {(approvalDto.ApprovalStatus == "Approved" ? "الموافقة على" : "رفض")} الملف الشخصي بنجاح", result));
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"خطأ في {(approvalDto.ApprovalStatus == "Approved" ? "الموافقة على" : "رفض")} الملف الشخصي {id}");
        return StatusCode(500, new ApiResponse<ProfileDto>(false, "خطأ في الخادم الداخلي", null));
    }
}




        [Authorize(Roles = "Admin")]

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteProfile(Guid id)
        {
            try
            {
                var result = await _context.Profiles.Include(p => p.Degrees).Where(p => p.ID == id).ToListAsync();

                var profile = await _context.Profiles.FindAsync(id);


                if (profile == null)
                    return NotFound(new ApiResponse<string>(false, "Profile not found", null));

                // Delete associated photo if exists
                if (!string.IsNullOrEmpty(profile.Photo))
                {
                    var oldPhotoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile", profile.Photo);
                    if (System.IO.File.Exists(oldPhotoPath))
                        System.IO.File.Delete(oldPhotoPath);
                }

                // Delete associated CV file if exists
                if (!string.IsNullOrEmpty(profile.Cv_File))
                {
                    var oldCvFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile", profile.Cv_File);
                    if (System.IO.File.Exists(oldCvFilePath))
                        System.IO.File.Delete(oldCvFilePath);
                }

                // Delete the profile from the database
                _context.Profiles.Remove(profile);
                _context.Degrees.RemoveRange(result.SelectMany(p => p.Degrees));
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<string>(true, "Profile and associated files deleted successfully", null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting profile {id}");
                return StatusCode(500, new ApiResponse<string>(false, "Internal server error", null));
            }
        }

    }
}





















