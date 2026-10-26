using System.Collections.Generic;

namespace MovieTicketBooking.Application.DTOs.Customer.Show
{
    public class CustomerRowDto
    {
        public int RowId { get; set; }
        public string RowName { get; set; } = string.Empty;
        public string RowType { get; set; } = string.Empty; // Normal, Walkway
        public int DisplayOrder { get; set; }
        public List<CustomerSeatDto> Seats { get; set; } = new List<CustomerSeatDto>();
    }
}
