using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.TheaterCreation;
using MovieTicketBookingApplication.Interfaces.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketBooking.Pages.SuperAdmin.Theater
{
    [Authorize(Roles = "SuperAdmin")]
    public class DetailsModel : PageModel
    {
        private readonly ITheaterService _theaterService;
        private readonly IScreenService _screenService;

        public DetailsModel(ITheaterService theaterService, IScreenService screenService)
        {
            _theaterService = theaterService;
            _screenService = screenService;
        }

        public CreateTheater? Theater { get; set; }

        public List<ScreenCardDto> Screens { get; set; } = new();

        [BindProperty]
        public CreateTheater EditTheater { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id <= 0) return RedirectToPage("/SuperAdmin/Theater/TheaterListing");

            Theater = await _theaterService.GetTheaterByIdAsync(id);
            if (Theater == null) return RedirectToPage("/SuperAdmin/Theater/TheaterListing");

            EditTheater = Theater;
            Screens = await _screenService.GetScreensByTheaterIdAsync(id);

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateTheaterAsync()
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
                bool result = await _theaterService.UpdateTheaterAsync(EditTheater);
                if (!result)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        errors = new Dictionary<string, string[]>
                        {
                            { string.Empty, new[] { "Unable to update theater details." } }
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
