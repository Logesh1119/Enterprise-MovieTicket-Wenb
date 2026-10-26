using System.Collections.Generic;

namespace MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen
{
    public class ScreenCardDto
    {
        public int ScreenId { get; set; }

        public int TheaterId { get; set; }

        public string ScreenName { get; set; } = string.Empty;

        public string ScreenCode { get; set; } = string.Empty;

        public string Floor { get; set; } = string.Empty;

        public string ScreenType { get; set; } = string.Empty;

        public string AudioTechnology { get; set; } = string.Empty;

        public string SeatingType { get; set; } = string.Empty;

        public int TotalRows { get; set; }

        public int TotalSeats { get; set; }

        public bool Status { get; set; }

        public string? AmenitiesString { get; set; }

        public List<string> Amenities { get; set; } = new();
    }
}
