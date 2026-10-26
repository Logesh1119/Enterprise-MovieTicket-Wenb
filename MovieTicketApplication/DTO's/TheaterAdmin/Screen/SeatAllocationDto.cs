using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen
{
    public class SeatAllocationDto
    {
        public int ScreenRowId { get; set; }

        public int? SeatCategoryId { get; set; }
    }
}
