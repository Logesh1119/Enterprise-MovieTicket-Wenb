using System.Collections.Generic;

namespace MovieTicketBooking.Application.DTOs.Customer.Booking
{
    public class CreateBookingDto
    {
        public int ShowId { get; set; }
        public List<int> ScreenSeatIds { get; set; } = new List<int>();
    }
}
