using System.ComponentModel.DataAnnotations;

namespace Setup.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 40 characters.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 50 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+])[A-Za-z\d!@#$%^&*()_+]{8,50}$", ErrorMessage = "Password must include at least one capital letter, one number, and one special character.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Verify Password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? VerifyPassword { get; set; }
    }
}
