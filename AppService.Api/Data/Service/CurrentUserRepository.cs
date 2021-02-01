using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace AppService.Api.Data.Service
{
    public class CurrentUserRepository : ICurrentUserRepository
    {
        public CurrentUserRepository(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;

            if (user == null)
            {
                throw new InvalidOperationException("This request does not have an authenticated user.");
            }

            this.UserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public string UserId { get; }
    }
}
