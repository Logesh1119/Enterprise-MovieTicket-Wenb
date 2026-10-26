using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen
{

    public class ScreenLayoutDto
    {
        public int ScreenId { get; set; }

        public int TotalRows => Rows.Count;

        public int TotalSeats => Rows.Sum(r => r.TotalSeats);

        public List<ScreenRowDto> Rows { get; set; } = new();
    }
}

