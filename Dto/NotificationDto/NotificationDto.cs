using System.ComponentModel.DataAnnotations;

namespace Ai_LibraryApi.Dto
{
    public class NotificationDto
    {
        public Guid UniqueID { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsSeen { get; set; }
        public string FromType { get; set; }
        public Guid UserId { get; set; } = new Guid();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateNotificationDto
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsSeen { get; set; }
        public string FromType { get; set; }
        public Guid UserId { get; set; }=new Guid();
    }

    public class UpdateNotificationDto
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsSeen { get; set; }
        public string FromType { get; set; }
    }

    public class NotificationSearchParams
    {
        [Required(ErrorMessage ="This feild important")]
        public string Title { get; set; }
        public string FromType { get; set; }
        public int PageNumber { get; set; } 
        public int PageSize { get; set; } 
    }
}
