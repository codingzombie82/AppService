using AppService.Api.Data.Service;
using AppService.Api.Data.Service.Identity;
using AppService.Models.DTO.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppService.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityRepository identity;
        private readonly ICurrentUserRepository currentUser;

        public IdentityController(
            IIdentityRepository identity,
            ICurrentUserRepository currentUser)
        {
            this.identity = identity;
            this.currentUser = currentUser;
        }

        [HttpPost(nameof(Register))]
        public async Task<ActionResult> Register(RegisterRequestModel model)
        {
            return await this.identity
                .RegisterAsync(model)
                .ToActionResult();
        }

        [HttpPost(nameof(Login))]
        public async Task<ActionResult<LoginResponseModel>> Login(LoginRequestModel model)
        {
            return await this.identity.LoginAsync(model).ToActionResult();
        }

        [Authorize]
        [HttpPut(nameof(ChangeSettings))]
        public async Task<ActionResult> ChangeSettings(ChangeSettingsRequestModel model)
        {
            return await this.identity
                .ChangeSettingsAsync(model, this.currentUser.UserId)
                .ToActionResult();
        }

        [Authorize]
        [HttpPut(nameof(ChangePassword))]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequestModel model)
        {
            return await this.identity
                .ChangePasswordAsync(model, this.currentUser.UserId)
                .ToActionResult();

        }
    }
}
