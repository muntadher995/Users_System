using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ai_LibraryApi.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
 
using Ai_LibraryApi.Models;
using Ai_LibraryApi.Dto;
using Org.BouncyCastle.Bcpg;
using Microsoft.AspNetCore.Authorization;


namespace Ai_LibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly Ai_LibraryApiDbContext _context;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(Ai_LibraryApiDbContext context, ILogger<NotificationsController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<NotificationDto>>>>
            GetNotifications([FromQuery] string? search, int pageNumber = 1, int pageSize = 10)
        {
            try
            {


                var query = _context.Notifications.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var loweredSearch = search.Trim().ToLower();
                    query = query.Where(p =>
                        (p.Title != null && p.Title.Trim().ToLower().Contains(loweredSearch)) ||
                        (p.Title != null && p.Body.Trim().ToLower().Contains(loweredSearch))||
                        (p.FromType != null && p.FromType.Trim().ToLower().Contains(loweredSearch))
                    );
                }
                

 

                // Get total count before pagination
                var totalCount = await query.CountAsync();

                // Apply pagination
                var items = await query
                    .OrderByDescending(n => n.CreatedAt)
                     .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(n => new NotificationDto
                    {
                        UniqueID = n.UniqueID,
                        Title = n.Title,
                        Body = n.Body,
                        IsSeen = n.IsSeen,
                        FromType = n.FromType,
                        UserId = n.UserId,
                        CreatedAt = n.CreatedAt,
                        UpdatedAt = n.UpdatedAt
                    })
                    .ToListAsync();

                var result = new PagedResult<NotificationDto>(
                    items,
                    totalCount,
                   pageNumber,
                   pageSize
                );
                 
                 return Ok(new ApiResponse<PagedResult<NotificationDto>>(
                    true,
                    "Notifications retrieved successfully",
                    result
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notifications");
                return StatusCode(500, new ApiResponse<PagedResult<NotificationDto>>(
                    false,
                    "An error occurred while retrieving notifications",
                    null
                ));
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<NotificationDto>>> GetNotification(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting notification with ID: {Id}", id);

                var notification = await _context.Notifications.FindAsync(id);

                if (notification == null)
                {
                    _logger.LogWarning("Notification with ID {Id} not found", id);
                    return NotFound(new ApiResponse<NotificationDto>(
                        false,
                        $"Notification with ID {id} not found",
                        null
                    ));
                }

                var notificationDto = new NotificationDto
                {
                    UniqueID = notification.UniqueID,
                    Title = notification.Title,
                    Body = notification.Body,
                    IsSeen = notification.IsSeen,
                    FromType = notification.FromType,
                    UserId = notification.UserId,
                    CreatedAt = notification.CreatedAt,
                    UpdatedAt = notification.UpdatedAt
                };

                return Ok(new ApiResponse<NotificationDto>(
                    true,
                    "Notification retrieved successfully",
                    notificationDto
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notification with ID {Id}", id);
                return StatusCode(500, new ApiResponse<NotificationDto>(
                    false,
                    "An error occurred while retrieving the notification",
                    null
                ));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<NotificationDto>>> CreateNotification(CreateNotificationDto createDto)
        {
            try
            {
                _logger.LogInformation("Creating new notification: {@CreateDto}", createDto);
               //ar userid = _context.Notifications
                var notification = new Notification
                {
                    UniqueID = Guid.NewGuid(),
                    Title = createDto.Title,
                    Body = createDto.Body,
                    IsSeen = createDto.IsSeen,
                    FromType = createDto.FromType,
                    UserId = createDto.UserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                var userid = _context.Notifications.Select(n => n.UserId).FirstOrDefault();
                if(createDto.UserId!= userid)
                {
                    return BadRequest(new ApiResponse<NotificationDto>(
                        false,
                        "User ID does not match",
                        null
                    ));
                }


                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                var notificationDto = new NotificationDto
                {
                    UniqueID = notification.UniqueID,
                    Title = notification.Title,
                    Body = notification.Body,
                    IsSeen = notification.IsSeen,
                    FromType = notification.FromType,
                    UserId = notification.UserId,
                    CreatedAt = notification.CreatedAt,
                    UpdatedAt = notification.UpdatedAt
                };

                return CreatedAtAction(
                    nameof(GetNotification),
                    new { id = notification.UniqueID },
                    new ApiResponse<NotificationDto>(
                        true,
                        "Notification created successfully",
                        notificationDto
                    )
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                return StatusCode(500, new ApiResponse<NotificationDto>(
                    false,
                     
                   ex.Message,
                   null
                    
                    
                ));
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<NotificationDto>>> UpdateNotification(Guid id, UpdateNotificationDto updateDto)
        {
            try
            {
                _logger.LogInformation("Updating notification with ID {Id}: {@UpdateDto}", id, updateDto);

                var notification = await _context.Notifications.FindAsync(id);

                if (notification == null)
                {
                    _logger.LogWarning("Notification with ID {Id} not found for update", id);
                    return NotFound(new ApiResponse<NotificationDto>(
                        false,
                        $"Notification with ID {id} not found",
                        null
                    ));
                }

                notification.Title = updateDto.Title ?? notification.Title;
                notification.Body = updateDto.Body ?? notification.Title;
                notification.IsSeen = updateDto.IsSeen;
                notification.FromType = updateDto.FromType ?? notification.Title;
                notification.UpdatedAt = DateTime.UtcNow;

                _context.Entry(notification).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var notificationDto = new NotificationDto
                {
                    UniqueID = notification.UniqueID,
                    Title = notification.Title,
                    Body = notification.Body,
                    IsSeen = notification.IsSeen,
                    FromType = notification.FromType,
                    UserId = notification.UserId,
                    CreatedAt = notification.CreatedAt,
                    UpdatedAt = notification.UpdatedAt
                };

                return Ok(new ApiResponse<NotificationDto>(
                    true,
                    "Notification updated successfully",
                    notificationDto
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification with ID {Id}", id);
                return StatusCode(500, new ApiResponse<NotificationDto>(
                    false,
                    "An error occurred while updating the notification",
                    null
                ));
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteNotification(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting notification with ID {Id}", id);

                var notification = await _context.Notifications.FindAsync(id);
                if (notification == null)
                {
                    _logger.LogWarning("Notification with ID {Id} not found for deletion", id);
                    return NotFound(new ApiResponse<bool>(
                        false,
                        $"Notification with ID {id} not found",
                        false
                    ));
                }

                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<bool>(
                    true,
                    "Notification deleted successfully",
                    true
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification with ID {Id}", id);
                return StatusCode(500, new ApiResponse<bool>(
                    false,
                    "An error occurred while deleting the notification",
                    false
                ));
            }
        }

      
    }
}