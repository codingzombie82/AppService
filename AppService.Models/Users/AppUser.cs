using Microsoft.AspNetCore.Identity;
using System;

namespace AppService.Models.Users
{
    public class AppUser : IdentityUser
    {
        public AppUser() => this.Id = Guid.NewGuid().ToString();

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
