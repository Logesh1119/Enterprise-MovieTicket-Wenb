using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MovieTicketBookingApplication.DTO_s.TheaterAdmin.Movies
{
    public class CreateMovieRequest
    {

        public int TheaterId { get; set; }
        [Required]
        public string MovieName { get; set; } = string.Empty;

        [Required]
        public string MovieCode { get; set; } = string.Empty;
        [Required]
        public string MovieDescription { get; set; } = string.Empty;
        public bool Status { get; set; }

        // Information
        [Required]
        public string Tags { get; set; } = string.Empty;

        public string TrailerUrl { get; set; } = string.Empty;

        public string TeaserUrl { get; set; } = string.Empty;

        public int Duration { get; set; }

        public DateTime ReleaseDate { get; set; }

        [Required]
        public int CertificateId { get; set; }

        // Multiple Images
        public List<IFormFile> Images { get; set; } = new();

        public List<int> GenreId { get; set; } = new();
    }
}
