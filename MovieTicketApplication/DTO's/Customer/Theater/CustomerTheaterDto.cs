using System.Collections.Generic;

namespace MovieTicketBooking.Application.DTOs.Customer.Theater
{
    public class CustomerTheaterDto
    {
        public int TheaterId { get; set; }
        public string TheaterName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        // The specific MovieId for THIS theater that matches the movie name
        public int LocalMovieId { get; set; }
        public List<CustomerShowDto> Shows { get; set; } = new List<CustomerShowDto>();
    }
}
