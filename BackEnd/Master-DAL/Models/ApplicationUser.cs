using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        //Navigation Property
        public ICollection<Articles> Articles { get; set; }
        public ICollection<Comments> Comments { get; set; }
        public ICollection<Likes> Likes { get; set; }

        public ICollection<UserPermission> UserPermissions { get; set; }
    }
}
