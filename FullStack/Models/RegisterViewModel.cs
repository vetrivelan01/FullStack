using System;
using System.ComponentModel.DataAnnotations;

namespace FullStack.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Login ID (Username)")]
        [StringLength(50, MinimumLength = 3)]
        public string LoginID { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirm password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Full Name")]
        public string CitizenName { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [FullStack.Helpers.MinimumAge(18, ErrorMessage = "You must be at least 18 years old to register.")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Nationality { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
