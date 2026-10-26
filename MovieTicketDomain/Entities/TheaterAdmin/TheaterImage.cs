using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketBookingDomain.Entities.TheaterAdmin
{
    public class TheaterImage
    {
        public int Id { get; set; }
        public int TheaterId { get; set; }
        public int MediaId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; }

    }
}
