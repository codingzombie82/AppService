using Microsoft.AspNetCore.Identity;
using System;

namespace AppService.Models.Users
{
    public class AppRole : IdentityRole
    {
        public AppRole()
            : this(null)
        {
        }

        public AppRole(string name)
        { 
            this.Id = Guid.NewGuid().ToString();
            this.Name = name;
        }
    }
}
