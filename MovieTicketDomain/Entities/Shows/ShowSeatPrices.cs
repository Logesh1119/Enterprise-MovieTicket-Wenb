using System;

namespace MovieTicketBookingDomain.Entities.Shows
{
    public class ShowSeatPrices
    {
        public int Id { get; set; }
        public int ShowId { get; set; }
        public int SeatCategoryId { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
