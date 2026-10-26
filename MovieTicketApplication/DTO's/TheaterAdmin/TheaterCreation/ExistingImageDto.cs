using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketBookingApplication.DTO_s.TheaterAdmin.TheaterCreation
{
    public class ExistingImageDto
    {
        public int MediaId { get; set; }

        public string FilePath { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }
    }

}
