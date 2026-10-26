namespace MovieTicketBooking.Domain.Entities
{
    public class TheaterAdminProfile
    {
        public int Id { get; set; }

        // Foreign Key to Users table
        public int UserId { get; set; }

        public int? ProfileImageId { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public bool Status { get; set; } = true;

        public DateTime CreatedAt { get; set; }
    }
}