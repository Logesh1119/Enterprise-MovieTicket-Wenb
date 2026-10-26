using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Show;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBookingApplication.Interfaces.Movies;
using MovieTicketBookingApplication.Interfaces.Screen;
using MovieTicketBookingApplication.Interfaces.Show;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieTicketBooking.Pages.SuperAdmin.Show
{
    [IgnoreAntiforgeryToken(Order = 1000)]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin")]
    public class ShowListingModel : PageModel
    {
        private readonly IShowService _showService;
        private readonly ITheaterService _theaterService;
        private readonly IScreenService _screenService;
        private readonly IMovieService _movieService;

        public ShowListingModel(
            IShowService showService,
            ITheaterService theaterService,
            IScreenService screenService,
            IMovieService movieService)
        {
            _showService = showService;
            _theaterService = theaterService;
            _screenService = screenService;
            _movieService = movieService;
        }

        public List<dynamic> Theaters { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public int? SelectedTheaterId { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? SelectedScreenId { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public DateTime? SelectedDate { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int userId = 0;
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

            // Fetch theaters for dropdown
            var theaters = await _theaterService.GetTheatersAsync();
            Theaters = theaters.Select(t => (dynamic)new { Id = t.TheaterId, Name = t.TheaterName }).ToList();
            
            if (!SelectedTheaterId.HasValue && Theaters.Any())
            {
                SelectedTheaterId = Theaters.First().Id;
            }
            
            if (!SelectedDate.HasValue)
            {
                SelectedDate = DateTime.Today;
            }

            return Page();
        }

        // AJAX handler to get screens for a theater
        public async Task<IActionResult> OnGetScreensAjaxAsync(int theaterId)
        {
            var screens = await _screenService.GetScreensByTheaterIdAsync(theaterId);
            return new JsonResult(screens.Select(s => new {
                id = s.ScreenId,
                name = s.ScreenName,
                type = s.ScreenType,
                audio = s.AudioTechnology,
                seats = s.TotalSeats
            }));
        }

        // AJAX handler to get shows for a screen and date
        public async Task<IActionResult> OnGetShowsAjaxAsync(int screenId, string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
            {
                var shows = await _showService.GetShowsListAsync(screenId, parsedDate);
                return new JsonResult(shows);
            }
            return BadRequest();
        }

        // AJAX handler to get available movies
        public async Task<IActionResult> OnGetMoviesAjaxAsync(int theaterId)
        {
            var movies = await _movieService.GetMoviesAsync();
            var filtered = movies.Where(m => m.TheaterId == theaterId).ToList();
            return new JsonResult(filtered.Select(m => new { id = m.MovieId, name = m.MovieName }));
        }

        // AJAX handler to get default prices for screen
        public async Task<IActionResult> OnGetDefaultPricesAjaxAsync(int screenId)
        {
            var prices = await _showService.GetDefaultSeatPricesForScreenAsync(screenId);
            return new JsonResult(prices);
        }

        // AJAX handler to create show
        public async Task<IActionResult> OnPostCreateShowAjaxAsync([FromBody] CreateShowDto request)
        {
            if (request == null) return BadRequest();
            var success = await _showService.CreateShowAsync(request);
            return new JsonResult(new { success });
        }

        // AJAX handler to get a show for edit
        public async Task<IActionResult> OnGetShowForEditAjaxAsync(int showId)
        {
            var show = await _showService.GetShowForEditAsync(showId);
            if (show == null) return NotFound();
            return new JsonResult(show);
        }

        // AJAX handler to update show
        public async Task<IActionResult> OnPostUpdateShowAjaxAsync([FromBody] CreateShowDto request)
        {
            if (request == null) return BadRequest();
            var success = await _showService.UpdateShowAsync(request);
            return new JsonResult(new { success });
        }

        // AJAX handler to delete show
        public async Task<IActionResult> OnPostDeleteShowAjaxAsync(int showId)
        {
            var success = await _showService.DeleteShowAsync(showId);
            return new JsonResult(new { success });
        }
    }
}
