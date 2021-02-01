using AppService.Models.Contants;
using System.ComponentModel.DataAnnotations;

namespace AppService.Models.DTO.Users
{
    public class ChangePasswordRequestModel
    {
        [Required]
        [MinLength(IdentityConst.MinPasswordLength)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [MinLength(IdentityConst.MinPasswordLength)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [MinLength(IdentityConst.MinPasswordLength)]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; }
    }
}
