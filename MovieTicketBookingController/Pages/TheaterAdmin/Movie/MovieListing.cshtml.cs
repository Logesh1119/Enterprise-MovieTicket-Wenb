using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Movies;
using MovieTicketBookingApplication.Interfaces.Movies;
using System.Security.Claims;

namespace MovieTicketBooking.Pages.TheaterAdmin.Movie
{
    public class MovieListingModel : PageModel
    {
        private readonly IMovieService _movieService;

        public MovieListingModel(IMovieService movieService)
        {
            _movieService = movieService;
        }

        public List<MovieCardDto> Movies { get; set; } = new();

        [BindProperty]
        public CreateMovieRequest NewMovie { get; set; } = new();

        [BindProperty(Name = "EditMovieId")]
        public int EditMovieId { get; set; }

        public List<GenreDto> Genres { get; set; } = new();

        public List<CertificateDto> Certificates { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                Movies = await _movieService.GetMoviesAsync();
            }
            catch (UnauthorizedAccessException)
            {
                Movies = new();
            }

            Genres = await _movieService.GetGenresAsync();
            Certificates = await _movieService.GetCertificatesAsync();
        }

        public async Task<IActionResult> OnPostCreateMovieAjaxAsync()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kv => kv.Value != null && kv.Value.Errors.Count > 0)
                    .ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return new JsonResult(new { success = false, errors });
            }

            try
            {
                int userId = 0;
                int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

                bool result = await _movieService.CreateMovieAsync(NewMovie, userId);

                if (!result)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        errors = new Dictionary<string, string[]>
                        {
                            { string.Empty, new[] { "Unable to create movie. Please try again." } }
                        }
                    });
                }

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    errors = new Dictionary<string, string[]>
                    {
                        { string.Empty, new[] { ex.Message } }
                    }
                });
            }
        }

        public async Task<IActionResult> OnGetMovieForEditAjaxAsync(int movieId)
        {
            var movie = await _movieService.GetMovieByIdAsync(movieId);
            if (movie == null)
            {
                return new JsonResult(new { success = false, message = "Movie not found." });
            }
            return new JsonResult(new { success = true, data = movie });
        }
        
        public async Task<IActionResult> OnPostUpdateMovieAjaxAsync()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kv => kv.Value != null && kv.Value.Errors.Count > 0)
                    .ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return new JsonResult(new { success = false, errors });
            }

            try
            {
                bool result = await _movieService.UpdateMovieAsync(EditMovieId, NewMovie);

                if (!result)
                {
                    return new JsonResult(new { success = false, errors = new Dictionary<string, string[]> { { string.Empty, new[] { "Unable to update movie." } } } });
                }

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, errors = new Dictionary<string, string[]> { { string.Empty, new[] { ex.Message } } } });
            }
        }
    }
}
