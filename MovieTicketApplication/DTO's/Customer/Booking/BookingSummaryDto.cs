using System;
using System.Collections.Generic;

namespace MovieTicketBooking.Application.DTOs.Customer.Booking
{
    public class BookingSummaryDto
    {
        public int BookingId { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public string TheaterName { get; set; } = string.Empty;
        public string ScreenName { get; set; } = string.Empty;
        public DateTime ShowDate { get; set; }
        public TimeSpan ShowTime { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<string> SeatLabels { get; set; } = new List<string>();
    }
}
