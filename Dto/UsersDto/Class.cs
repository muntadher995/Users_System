 
    using System;

    namespace Ai_LibraryApi.API.DTOs
    {
        public class AdminDto
        {
            public Guid Id { get; set; }
            public string AdminName { get; set; }
            public string Email { get; set; }
            public string Photo { get; set; }
            public bool IsActive { get; set; }
        }

        public class RefreshTokenDto
        {
            public string RefreshToken { get; set; }
        }
    }
 