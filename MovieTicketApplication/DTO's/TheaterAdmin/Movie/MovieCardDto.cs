using System;

namespace MovieTicketBookingApplication.DTO_s.TheaterAdmin.Movies
{
    public class MovieCardDto
    {
        public int MovieId { get; set; }
        public int TheaterId { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public string MovieCode { get; set; } = string.Empty;
        public int Duration { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? ImagePath { get; set; }
        public string? CertificateName { get; set; }
        public bool Status { get; set; }
        public string? GenresString { get; set; }
        public List<string> Genres { get; set; } = new();
    }
}
