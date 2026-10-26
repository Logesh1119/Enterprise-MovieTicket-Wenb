using System.Collections.Generic;
using System.Threading.Tasks;
using MovieTicketBooking.Application.DTOs.Customer.Movie;

namespace MovieTicketBooking.Application.Interfaces.Customer
{
    public interface ICustomerMovieRepository
    {
        Task<IEnumerable<CustomerMovieDto>> GetFeaturedMoviesAsync();
        Task<IEnumerable<CustomerMovieDto>> GetUpcomingMoviesAsync();
        Task<IEnumerable<CustomerMovieDto>> GetAllMoviesAsync(string search, string genre);
        Task<CustomerMovieDetailDto> GetMovieByIdAsync(int id);
    }
}
