using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.DTOs.Customer.Movie;
using MovieTicketBooking.Application.DTOs.Customer.Theater;
using MovieTicketBooking.Application.Interfaces.Customer;

namespace MovieTicketBooking.Pages.Customer.Movies
{
    public class DetailsModel : PageModel
    {
        private readonly ICustomerMovieService _movieService;
        private readonly ICustomerTheaterService _theaterService;

        public DetailsModel(ICustomerMovieService movieService, ICustomerTheaterService theaterService)
        {
            _movieService = movieService;
            _theaterService = theaterService;
        }

        public CustomerMovieDetailDto Movie { get; set; }
        public IEnumerable<CustomerTheaterDto> Theaters { get; set; } = new List<CustomerTheaterDto>();
        
        [BindProperty(SupportsGet = true)]
        public string SelectedDate { get; set; }

        public List<DateTime> AvailableDates { get; set; } = new List<DateTime>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Movie = await _movieService.GetMovieByIdAsync(id);
            if (Movie == null) return NotFound();

            // Generate next 5 days
            for (int i = 0; i < 5; i++)
            {
                AvailableDates.Add(DateTime.Today.AddDays(i));
            }

            DateTime queryDate;
            if (string.IsNullOrEmpty(SelectedDate) || !DateTime.TryParse(SelectedDate, out queryDate))
            {
                queryDate = DateTime.Today;
                SelectedDate = queryDate.ToString("yyyy-MM-dd");
            }

            Theaters = await _theaterService.GetTheatersPlayingMovieAsync(Movie.MovieName, queryDate);

            return Page();
        }
    }
}
