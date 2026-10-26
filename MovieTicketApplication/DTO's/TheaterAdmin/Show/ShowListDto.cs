using System;

namespace MovieTicketBookingApplication.DTO_s.TheaterAdmin.Show
{
    public class ShowListDto
    {
        public int Id { get; set; }
        public int ScreenId { get; set; }
        public string ScreenName { get; set; } = string.Empty;
        public int MovieId { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public string MovieImagePath { get; set; } = string.Empty;
        public DateTime ShowDate { get; set; }
        public TimeSpan ShowTime { get; set; }
        public string Language { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PriceRange { get; set; } = string.Empty;
        public int Bookings { get; set; }
    }
}
