using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketBookingDomain.Entities.Movies
{
    public class MovieImages
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int MediaId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; }

    }
}
