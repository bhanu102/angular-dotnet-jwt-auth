using System;
using System.Text.Json.Serialization;

namespace JwtDemo.API.Models
{
    public class RefreshToken
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Revoked { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsActive => Revoked == null && !IsExpired;

        public Guid UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
    }
}
