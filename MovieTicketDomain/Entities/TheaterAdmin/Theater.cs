using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketBookingDomain.Entities.TheaterAdmin
{
    public class Theater
    {
        public int Id { get; set; }
        public int OwnerUserId { get; set; }
        public string TheaterName { get; set; }
        public string TheaterCode { get; set; }
        public string TheaterBrand { get; set; }
        public string TheaterDescription { get; set; }
        public bool Status { get; set; } = true;

    }
}
