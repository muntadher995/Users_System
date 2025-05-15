using Ai_LibraryApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ai_LibraryApi.Models
{
    public class Notification
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]

        public Guid UniqueID { get; set; }

        public string? Title { get; set; }

      
        public string? Body { get; set; }

        public bool IsSeen { get; set; }= false;

        public string? FromType { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }=new Guid();    

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}