using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.Application.DTOs
{
    public class CreateTheaterAdminRequest
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        public bool Status { get; set; } = true;

        public IFormFile? ProfileImage { get; set; }

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}