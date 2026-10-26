using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen
{
    public class ScreenRowAllocationDto
    {
        public int Id { get; set; }

        public string RowName { get; set; }

        public string RowType { get; set; }

        public int? SeatCategoryId { get; set; }
    }
}
