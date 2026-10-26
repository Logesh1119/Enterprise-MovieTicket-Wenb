namespace MovieTicketBooking.Application.DTOs.Customer.Show
{
    public class CustomerSeatDto
    {
        public int SeatId { get; set; }
        public int SeatNo { get; set; }
        public string SeatLabel { get; set; } = string.Empty;
        public string SeatType { get; set; } = string.Empty; // Seat, Walkway
        public int SeatCategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsBooked { get; set; }
    }
}
