using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen
{

    public class ScreenRowDto
    {
        public int Id { get; set; }
        public string RowName { get; set; } = string.Empty;

        public string RowType { get; set; } = string.Empty;

        public int TotalSeats { get; set; }

        public int WalkwayCount { get; set; }

        public int? SeatCategoryId { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsReverse { get; set; }

        public List<ScreenSeatDto> Seats { get; set; } = new();
    }
}


