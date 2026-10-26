using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.DTOs.Customer.Movie;
using MovieTicketBooking.Application.Interfaces.Customer;

namespace MovieTicketBooking.Pages.Customer.Movies
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerMovieService _movieService;

        public IndexModel(ICustomerMovieService movieService)
        {
            _movieService = movieService;
        }

        public IEnumerable<CustomerMovieDto> Movies { get; set; } = new List<CustomerMovieDto>();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Genre { get; set; }

        public async Task OnGetAsync()
        {
            Movies = await _movieService.GetAllMoviesAsync(SearchTerm, Genre);
        }
    }
}
