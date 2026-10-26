using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MovieTicketBooking.Application.DTOs.Customer.Theater;

namespace MovieTicketBooking.Application.Interfaces.Customer
{
    public interface ICustomerTheaterService
    {
        Task<IEnumerable<CustomerTheaterDto>> GetAllTheatersAsync();
        Task<IEnumerable<CustomerTheaterDto>> GetTheatersPlayingMovieAsync(string movieName, DateTime date);
    }
}
