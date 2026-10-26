using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen
{
    public class CreateScreen
    {
        public int ScreenId { get; set; }
        public int TheaterId { get; set; }
        [Required]
        public string ScreenName { get; set; } = string.Empty;
        [Required]
        public string ScreenCode { get; set; } = string.Empty;
        [Required]
        public string Floor { get; set; } = string.Empty;
        [Required]
        public string ScreenType { get; set; } = string.Empty;
        [Required]
        public string AudioTechnology { get; set; } = string.Empty;
        [Required]
        public string SeatingType { get; set; } = string.Empty;

        public bool Status { get; set; } = true;

        public List<int> AmenId { get; set; } = new();

    }
}
