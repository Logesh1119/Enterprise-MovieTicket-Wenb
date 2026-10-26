using System;

namespace MovieTicketBookingDomain.Entities.Booking
{
    public class BookingSeats
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int ScreenSeatId { get; set; }
        public decimal Price { get; set; }
    }
}
