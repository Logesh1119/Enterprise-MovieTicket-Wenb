using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketBookingDomain.Entities.Screen
{
    public class ScreenSeats
    {
        public int Id { get; set; }

        public int ScreenRowId { get; set; }

        public int SeatNo { get; set; }

        public string SeatLabel { get; set; } = string.Empty;

        public string SeatType { get; set; } = string.Empty;

        public bool Status { get; set; }
    }
}

