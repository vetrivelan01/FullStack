using System.ComponentModel.DataAnnotations;

namespace FullStack.Models
{
    public class Citizen
    {
        [Key]
        public string CitizenID { get; set; } = string.Empty;

        [Required]
        public string CitizenName { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public string Nationality { get; set; } = string.Empty;

        [Required]
        public string Language { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}