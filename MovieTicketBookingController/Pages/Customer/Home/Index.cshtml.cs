using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.DTOs.Customer.Movie;
using MovieTicketBooking.Application.Interfaces.Customer;

namespace MovieTicketBooking.Pages.Customer.Home
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerMovieService _movieService;

        public IndexModel(ICustomerMovieService movieService)
        {
            _movieService = movieService;
        }

        public IEnumerable<CustomerMovieDto> FeaturedMovies { get; set; } = new List<CustomerMovieDto>();
        public IEnumerable<CustomerMovieDto> UpcomingMovies { get; set; } = new List<CustomerMovieDto>();

        public async Task OnGetAsync()
        {
            FeaturedMovies = await _movieService.GetFeaturedMoviesAsync();
            UpcomingMovies = await _movieService.GetUpcomingMoviesAsync();
        }
    }
}
