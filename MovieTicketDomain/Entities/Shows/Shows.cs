using System;

namespace MovieTicketBookingDomain.Entities.Shows
{
    public class Shows
    {
        public int Id { get; set; }
        public int ScreenId { get; set; }
        public int MovieId { get; set; }
        public DateTime ShowDate { get; set; }
        public TimeSpan ShowTime { get; set; }
        public string Language { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
