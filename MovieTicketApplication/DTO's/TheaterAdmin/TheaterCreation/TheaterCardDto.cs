namespace MovieTicketBookingApplication.DTO_s.TheaterAdmin.Theater
{
    public class TheaterCardDto
    {
        public int TheaterId { get; set; }
        public int OwnerUserId { get; set; }

        public string TheaterName { get; set; } = string.Empty;

        public string TheaterBrand { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string? ImagePath { get; set; }

        public bool Status { get; set; }

        public string? AmenitiesString { get; set; }


        public List<string> Amenities { get; set; } = new();

        public string TheaterCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string Area { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
    }
}