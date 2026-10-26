using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketBookingDomain.Entities.Screen
{
    public class ScreenRows
    {
        public int Id { get; set; }

        public int ScreenId { get; set; }

        public string RowName { get; set; } = string.Empty;

        public string RowType { get; set; } = string.Empty;

        public int TotalSeats { get; set; }

        public int WalkwayCount { get; set; }

        public int? SeatCategoryId { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsReverse { get; set; }
    }
}

