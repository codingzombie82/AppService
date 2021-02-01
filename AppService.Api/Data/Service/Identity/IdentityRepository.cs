﻿using AppService.Models.DTO;
using AppService.Models.DTO.Users;
using AppService.Models.Users;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace AppService.Api.Data.Service.Identity
{
    public class IdentityRepository : IIdentityRepository
    {
        private const string InvalidErrorMessage = "Invalid email or password.";

        private readonly UserManager<AppUser> userManager;
        private readonly IJwtGeneratorService jwtGenerator;

        public IdentityRepository(
            UserManager<AppUser> userManager,
            IJwtGeneratorService jwtGenerator)
        {
            this.userManager = userManager;
            this.jwtGenerator = jwtGenerator;
        }

        public async Task<Result> RegisterAsync(RegisterRequestModel model)
        {
            var user = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email
            };

            var identityResult = await this.userManager.CreateAsync(user, model.Password);

            var errors = identityResult.Errors.Select(e => e.Description);

            return identityResult.Succeeded
                ? Result.Success
                : Result.Failure(errors);
        }

        public async Task<Result<LoginResponseModel>> LoginAsync(LoginRequestModel model)
        {
            var user = await this.userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return InvalidErrorMessage;
            }

            var passwordValid = await this.userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                return InvalidErrorMessage;
            }

            var token = await this.jwtGenerator.GenerateJwtAsync(user);

            return new LoginResponseModel { Token = token };
        }

        public async Task<Result> ChangeSettingsAsync(
            ChangeSettingsRequestModel model, string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return InvalidErrorMessage;
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var identityResult = await this.userManager.UpdateAsync(user);

            var errors = identityResult.Errors.Select(e => e.Description);

            return identityResult.Succeeded
                ? Result.Success
                : Result.Failure(errors);
        }

        public async Task<Result> ChangePasswordAsync(
            ChangePasswordRequestModel model, string userId)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return InvalidErrorMessage;
            }

            var identityResult = await this.userManager.ChangePasswordAsync(
                user,
                model.Password,
                model.NewPassword);

            var errors = identityResult.Errors.Select(e => e.Description);

            return identityResult.Succeeded
                ? Result.Success
                : Result.Failure(errors);
        }
    }
}
