using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ResNet.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;

        [MaxLength(300)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginAt { get; set; }

        public List<Order> Orders { get; set; } = new();

        public List<ActionLog> ActionLogs { get; set; } = new();
    }
}
