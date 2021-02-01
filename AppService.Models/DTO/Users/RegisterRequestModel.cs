using AppService.Models.Contants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AppService.Models.DTO.Users
{
    public class RegisterRequestModel : LoginRequestModel
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

        [Required]
        [MinLength(IdentityConst.MinPasswordLength)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = ErrorMessages.PasswordsDoNotMatchErrorMessage)]
        public string ConfirmPassword { get; set; }
    }
}
