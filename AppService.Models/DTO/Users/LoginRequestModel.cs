using AppService.Models.Contants;
using System.ComponentModel.DataAnnotations;

namespace AppService.Models.DTO.Users
{
    public class LoginRequestModel
    {
        [Required]
        [EmailAddress]
        [MinLength(IdentityConst.MinEmailLength)]
        [MaxLength(IdentityConst.MaxEmailLength)]
        public string Email { get; set; }

        [Required]
        [MinLength(IdentityConst.MinPasswordLength)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
