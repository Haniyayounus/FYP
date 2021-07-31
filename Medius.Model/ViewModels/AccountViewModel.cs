using Medius.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Medius.Model.ViewModels
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class SetPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class AuthenticateResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Cnic { get; set; }
        public Role Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsVerified { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }
        public string ImagePath { get; set; }
        public bool AcceptTerms { get; set; }
    }
    public class AuthenticateRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string UserName { get; set; }
        public Role Role { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
       
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string CNIC { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Range(typeof(bool), "true", "true")]
        public bool AcceptTerms { get; set; }
    }
 
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string mode { get; set; }
    }
    public class ValidateResetTokenRequest
    {
            [Required]
            public string Token { get; set; }
    }

    public class ResetPasswordRequest
    {
    [Required]
    public string Token { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }
}

    public class AccountResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CNIC { get; set; }
        public string Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsVerified { get; set; }
    }
    public class CreateRequest
    {
        public string UserName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string CNIC { get; set; }

        [Required]
        [EnumDataType(typeof(Role))]
        public string Role { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
    public class UpdateRequest
    {
            private string _password;
            private string _confirmPassword;
            private string _role;
            private string _email;

            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PhoneNumber { get; set; }

        [EnumDataType(typeof(Role))]
            public string Role
            {
                get => _role;
                set => _role = replaceEmptyWithNull(value);
            }

            [EmailAddress]
            public string Email
            {
                get => _email;
                set => _email = replaceEmptyWithNull(value);
            }

            [MinLength(6)]
            public string Password
            {
                get => _password;
                set => _password = replaceEmptyWithNull(value);
            }

            [Compare("Password")]
            public string ConfirmPassword
            {
                get => _confirmPassword;
                set => _confirmPassword = replaceEmptyWithNull(value);
            }

            // helpers

            private string replaceEmptyWithNull(string value)
            {
                // replace empty string with null to make field optional
                return string.IsNullOrEmpty(value) ? null : value;
            }
        }
    public class RevokeTokenRequest
    {
        public string Token { get; set; }
    }
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
    public class ChangePassword
    {
        public string id { get; set; }
        public string password { get; set; }
        public string newPassword { get; set; }
    }
}
