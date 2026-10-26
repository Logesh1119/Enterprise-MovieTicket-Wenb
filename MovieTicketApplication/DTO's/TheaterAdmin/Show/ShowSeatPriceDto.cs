namespace MovieTicketBookingApplication.DTO_s.TheaterAdmin.Show
{
    public class ShowSeatPriceDto
    {
        public int SeatCategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
