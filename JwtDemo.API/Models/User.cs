using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JwtDemo.API.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public string PasswordHash { get; set; } = string.Empty;
        
        public DateTime MemberSince { get; set; } = DateTime.UtcNow;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
