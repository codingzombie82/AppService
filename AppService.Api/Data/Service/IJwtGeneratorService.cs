using AppService.Models.Users;
using System.Threading.Tasks;

namespace AppService.Api.Data.Service
{
    public interface IJwtGeneratorService
    {
        Task<string> GenerateJwtAsync(AppUser user);
    }
}
