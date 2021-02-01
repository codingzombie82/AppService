using AppService.Models.DTO;
using AppService.Models.DTO.Users;
using System.Threading.Tasks;

namespace AppService.Api.Data.Service.Identity
{
    public interface IIdentityRepository
    {
        Task<Result> RegisterAsync(RegisterRequestModel model);

        Task<Result<LoginResponseModel>> LoginAsync(LoginRequestModel model);

        Task<Result> ChangeSettingsAsync(ChangeSettingsRequestModel model, string userId);

        Task<Result> ChangePasswordAsync(ChangePasswordRequestModel model, string userId);
    }
}
