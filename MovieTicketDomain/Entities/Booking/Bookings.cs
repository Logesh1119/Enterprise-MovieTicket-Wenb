using System;
using System.Collections.Generic;

namespace MovieTicketBookingDomain.Entities.Booking
{
    public class Bookings
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ShowId { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingType { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
