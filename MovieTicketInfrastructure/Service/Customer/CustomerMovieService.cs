using System.Collections.Generic;
using System.Threading.Tasks;
using MovieTicketBooking.Application.DTOs.Customer.Movie;
using MovieTicketBooking.Application.Interfaces.Customer;

namespace MovieTicketBooking.Infrastructure.Service.Customer
{
    public class CustomerMovieService : ICustomerMovieService
    {
        private readonly ICustomerMovieRepository _repository;

        public CustomerMovieService(ICustomerMovieRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CustomerMovieDto>> GetFeaturedMoviesAsync()
        {
            return await _repository.GetFeaturedMoviesAsync();
        }

        public async Task<IEnumerable<CustomerMovieDto>> GetUpcomingMoviesAsync()
        {
            return await _repository.GetUpcomingMoviesAsync();
        }

        public async Task<IEnumerable<CustomerMovieDto>> GetAllMoviesAsync(string search, string genre)
        {
            return await _repository.GetAllMoviesAsync(search, genre);
        }

        public async Task<CustomerMovieDetailDto> GetMovieByIdAsync(int id)
        {
            return await _repository.GetMovieByIdAsync(id);
        }
    }
}
