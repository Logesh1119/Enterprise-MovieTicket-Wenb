using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketBookingDomain.Entities.Screens
{
    public class Screens
    {
        public int TheaterId { get; set; }
        public string ScreenName { get; set; }
        public string ScreenCode { get; set; }
        public string Floor { get; set; }
        public string ScreenType { get; set; }
        public string AudioTechnology { get; set; }
        public string SeatingType { get; set; }
        public bool Status { get; set; }

    }
}
