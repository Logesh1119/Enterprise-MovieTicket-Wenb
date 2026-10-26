using MovieTicketBooking.Domain.Entities;
using MovieTicketBookingDomain.Entities;

namespace MovieTicketBooking.Application.DTOs
{
    public class LoginUserDto
    {
        public Users User { get; set; } = new Users();

        public string RoleName { get; set; } = string.Empty;
    }
}