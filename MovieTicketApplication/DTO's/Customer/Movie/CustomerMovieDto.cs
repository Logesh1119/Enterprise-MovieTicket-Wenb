using System;

namespace MovieTicketBooking.Application.DTOs.Customer.Movie
{
    public class CustomerMovieDto
    {
        public int Id { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public string MovieCode { get; set; } = string.Empty;
        public int Duration { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string CertificateName { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public string Genres { get; set; } = string.Empty; // Comma separated
    }
}
