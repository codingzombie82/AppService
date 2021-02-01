using AppService.Models;
using AppService.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AppService.Api.Data.Service
{
    public class JwtGeneratorService : IJwtGeneratorService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly ApplicationDbContext dbContext;

        public JwtGeneratorService(UserManager<AppUser> _userManager, ApplicationDbContext _dbContext)
        {
            this.userManager = _userManager;
            this.dbContext = _dbContext;
        }

        public async Task<string> GenerateJwtAsync(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            };

            var isAdministrator = await this.userManager.IsInRoleAsync(user, "Admin");

            if (isAdministrator)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }


            var secret = Encoding.UTF8.GetBytes("dsafa!sdfasdfsafdasdfasdfasfdasfsd");

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(secret),
                    SecurityAlgorithms.HmacSha256));

            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.WriteToken(token);

            return encryptedToken;
        }
    }
}
