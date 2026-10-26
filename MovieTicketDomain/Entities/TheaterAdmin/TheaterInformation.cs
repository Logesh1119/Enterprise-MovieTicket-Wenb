using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketBookingDomain.Entities.TheaterAdmin
{
    public class TheaterInformation
    {
        public int Id { get; set; }
        public int TheaterId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }

    }
}
