using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MovieTicketBookingApplication.DTO_s.TheaterAdmin.TheaterCreation
{
    public class CreateTheater
    {
        public int TheaterId { get; set; }

        public int OwnerUserId { get; set; }

        [Required]
        public string TheaterName { get; set; } = string.Empty;

        [Required]
        public string TheaterCode { get; set; } = string.Empty;

        [Required]
        public string TheaterBrand { get; set; } = string.Empty;

        [Required]
        public string TheaterDescription { get; set; } = string.Empty;

        public bool Status { get; set; } = true;

        [Required]
        public string AddressLine1 { get; set; } = string.Empty;

        public string? AddressLine2 { get; set; }

        [Required]
        public string Area { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        [Required]
        public string ZipCode { get; set; } = string.Empty;

        [Required]
        public string ContactName { get; set; } = string.Empty;

        [Required]
        public string ContactPhone { get; set; } = string.Empty;

        public List<IFormFile> Images { get; set; } = new();

        public List<ExistingImageDto> ExistingImages { get; set; } = new();

        public List<int> DeletedMediaIds { get; set; } = new();
    }
}

