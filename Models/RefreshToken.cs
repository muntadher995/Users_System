namespace UserSystem.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string TokenHash { get; set; } = default!;
        public string TokenSalt { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;

        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
    }

}
