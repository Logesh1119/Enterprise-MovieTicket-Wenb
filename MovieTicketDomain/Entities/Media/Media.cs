namespace MovieTicketBooking.Domain.Entities
{
    public class Media
    {
        public int Id { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;
    }
}