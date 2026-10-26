using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketBookingDomain.Entities.Movies
{
    public class MoviesDb
    {
        public int Id { get; set; }
        public int TheaterId { get; set; }
        public string MovieName { get; set; }
        public string MovieCode { get; set; }
        public string MovieDescription { get; set; }
        public string TrailerUrl { get; set; }
        public string TeaserUrl { get; set; }
        public int Duration { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int CertificateId { get; set; }
        public bool Status { get; set; }
        public string Tags { get; set; }

    }
}
