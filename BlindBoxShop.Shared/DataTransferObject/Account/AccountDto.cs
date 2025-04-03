using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Shared.DataTransferObject.Account
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName => $"{FirstName} {LastName}";
        public string? Address { get; set; }
        public string? Provinces { get; set; }
        public string? District { get; set; }
        public string? Wards { get; set; }
        public string? Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsActive { get; set; }
        public string? LastLoginAt { get; set; }
    }
}
