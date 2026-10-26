using System.Collections.Generic;

namespace MovieTicketBooking.Application.DTOs.Customer.Show
{
    public class CustomerSeatLayoutDto
    {
        public int ShowId { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public string TheaterName { get; set; } = string.Empty;
        public string ScreenName { get; set; } = string.Empty;
        public string ShowDate { get; set; } = string.Empty;
        public string ShowTime { get; set; } = string.Empty;
        public List<CustomerRowDto> Rows { get; set; } = new List<CustomerRowDto>();
    }
}
