using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen
{
    public class ScreenSeatDto
    {
        public int SeatNo { get; set; }

        public string SeatLabel { get; set; } = string.Empty;

        public string SeatType { get; set; } = string.Empty;
    }
}


