using System;
namespace MovieTicketBookingApplication.DTO_s.TheaterAdmin.Settings
{
    public class ProfileDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? ProfileImagePath { get; set; }
    }
}
