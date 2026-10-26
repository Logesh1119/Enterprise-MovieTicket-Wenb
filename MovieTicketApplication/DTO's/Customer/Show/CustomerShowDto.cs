using System;

namespace MovieTicketBooking.Application.DTOs.Customer.Theater
{
    public class CustomerShowDto
    {
        public int ShowId { get; set; }
        public TimeSpan ShowTime { get; set; }
        public string Language { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string ScreenName { get; set; } = string.Empty;
        public int ScreenId { get; set; }
    }
}
