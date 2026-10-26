using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MovieTicketBooking.Application.DTOs.Customer.Theater;
using MovieTicketBooking.Application.Interfaces.Customer;

namespace MovieTicketBooking.Infrastructure.Service.Customer
{
    public class CustomerTheaterService : ICustomerTheaterService
    {
        private readonly ICustomerTheaterRepository _repository;

        public CustomerTheaterService(ICustomerTheaterRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CustomerTheaterDto>> GetAllTheatersAsync()
        {
            return await _repository.GetAllTheatersAsync();
        }

        public async Task<IEnumerable<CustomerTheaterDto>> GetTheatersPlayingMovieAsync(string movieName, DateTime date)
        {
            return await _repository.GetTheatersPlayingMovieAsync(movieName, date);
        }
    }
}
