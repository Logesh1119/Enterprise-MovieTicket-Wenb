using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Theater;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.TheaterCreation;

namespace MovieTicketBooking.Pages.TheaterAdmin.Theater
{
    public class TheaterListModel : PageModel
    {
        private readonly ITheaterService _theaterService;

        public TheaterListModel(ITheaterService theaterService)
        {
            _theaterService = theaterService;
        }

        public List<TheaterCardDto> Theaters { get; set; } = new();

        // ── Drawer form binding ──────────────────────────────────────────
        [BindProperty]
        public CreateTheater NewTheater { get; set; } = new();

        // ── GET ──────────────────────────────────────────────────────────
        public async Task OnGetAsync()
        {
            Theaters = await _theaterService.GetTheatersAsync();
        }

        // ── POST (AJAX / jQuery) — returns JSON ───────────────────────────
        public async Task<IActionResult> OnPostCreateTheaterAjaxAsync()
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
                bool result = await _theaterService.CreateTheaterAsync(NewTheater);

                if (!result)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        errors = new Dictionary<string, string[]>
                        {
                            { string.Empty, new[] { "Unable to create theater. Please try again." } }
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

        // ── POST (AJAX) — Update existing theater ─────────────────────────
        public async Task<IActionResult> OnPostUpdateTheaterAjaxAsync()
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
                bool result = await _theaterService.UpdateTheaterAsync(NewTheater);

                if (!result)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        errors = new Dictionary<string, string[]>
                        {
                            { string.Empty, new[] { "Unable to update theater. Please try again." } }
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
    }
}