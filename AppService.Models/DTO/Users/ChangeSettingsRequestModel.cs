using AppService.Models.Contants;
using System.ComponentModel.DataAnnotations;

namespace AppService.Models.DTO.Users
{
    public class ChangeSettingsRequestModel
    {
        [Required]
        [StringLength(
           CommonConst.MaxNameLength,
           ErrorMessage = ErrorMessages.StringLengthErrorMessage,
           MinimumLength = CommonConst.MinNameLength)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(
            CommonConst.MaxNameLength,
            ErrorMessage = ErrorMessages.StringLengthErrorMessage,
            MinimumLength = CommonConst.MinNameLength)]
        public string LastName { get; set; }
    }
}
