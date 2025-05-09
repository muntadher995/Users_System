using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ai_LibraryApi.Models
{
    public class Ads
    {
        public Guid UniqueID { get; set; }
        public string Url { get; set; }
        public string AdsLocation { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
