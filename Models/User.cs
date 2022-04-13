using Microsoft.AspNetCore.Identity;
using System;

namespace UserManagement.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLogin { get; set; }
        public string Status { get; set; }
    }
}
