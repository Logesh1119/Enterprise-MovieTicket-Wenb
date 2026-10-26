using System;
using System.Collections.Generic;

namespace MovieTicketBookingApplication.DTO_s.TheaterAdmin.Show
{
    public class CreateShowDto
    {
        public int Id { get; set; }
        public int ScreenId { get; set; }
        public int MovieId { get; set; }
        public DateTime ShowDate { get; set; }
        public TimeSpan ShowTime { get; set; }
        public string Language { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string Status { get; set; } = "Draft";
        public List<ShowSeatPriceDto> Prices { get; set; } = new();
    }
}
