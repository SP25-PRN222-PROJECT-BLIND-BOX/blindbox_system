using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Shared.DataTransferObject.Account
{
    public class AccountForUpdateDto
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; } = null!;

        public string? Address { get; set; }
        public string? Provinces { get; set; }
        public string? District { get; set; }
        public string? Wards { get; set; }
        public string? Image { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Staff|Manager)$", ErrorMessage = "Role must be either Staff or Manager")]
        public string Role { get; set; } = null!;

        public bool IsActive { get; set; }
    }
} 