using MovieTicketBookingDomain.Entities;

namespace MovieTicketBooking.Application.DTOs
{
    public class LoginResponse
    {
        public bool Success { get; set; }

        public string Token { get; set; } = string.Empty;

        public Users? User { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}