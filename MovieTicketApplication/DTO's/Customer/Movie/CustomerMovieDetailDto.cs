using System;

namespace MovieTicketBooking.Application.DTOs.Customer.Movie
{
    public class CustomerMovieDetailDto
    {
        public int Id { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public string MovieDescription { get; set; } = string.Empty;
        public string TrailerUrl { get; set; } = string.Empty;
        public int Duration { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string CertificateName { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public string BannerUrl { get; set; } = string.Empty;
        public string Genres { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
    }
}
