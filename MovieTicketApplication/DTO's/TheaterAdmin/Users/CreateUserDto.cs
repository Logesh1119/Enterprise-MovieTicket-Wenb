using System;
using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingApplication.DTO_s.TheaterAdmin.Users
{
    public class CreateUserDto
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        [Required]
        public Guid RoleId { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public bool Status { get; set; } = true;
    }
}
