using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketBookingDomain.Entities.Screen
{
    public class SeatCategories
    {
        public int Id { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string? ColorCode { get; set; }
    }
}

