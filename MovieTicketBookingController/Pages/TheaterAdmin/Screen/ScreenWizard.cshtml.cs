using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.TheaterCreation;
using MovieTicketBookingApplication.Interfaces.Screen;
using MovieTicketBookingDomain.Entities.Screen;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieTicketBooking.Pages.TheaterAdmin.Screen
{
    [IgnoreAntiforgeryToken(Order = 1000)]
    public class ScreenWizardModel : PageModel
    {
        private readonly IScreenService _screenService;
        private readonly ITheaterService _theaterService;

        public ScreenWizardModel(IScreenService screenService, ITheaterService theaterService)
        {
            _screenService = screenService;
            _theaterService = theaterService;
        }

        [BindProperty(SupportsGet = true)]
        public int TheaterId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? ScreenId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int InitialStep { get; set; } = 1;

        public CreateTheater? Theater { get; set; }

        public CreateScreen ScreenSpecs { get; set; } = new();

        public ScreenLayoutDto? ExistingLayout { get; set; }

        public List<CreateAmenity> Amenities { get; set; } = new();

        public List<SeatCategories> SeatCategories { get; set; } = new();

        public List<string> ScreenTypes { get; set; } = new()
        {
            "Standard", "IMAX", "Dolby Cinema", "4DX", "ScreenX"
        };

        public List<string> AudioTechnologies { get; set; } = new()
        {
            "Dolby Atmos", "Dolby Digital 7.1", "DTS:X", "Auro 3D"
        };

        public List<string> SeatingTypes { get; set; } = new()
        {
            "Standard", "Recliner", "Luxury Recliner"
        };

        public async Task<IActionResult> OnGetAsync()
        {
            if (TheaterId <= 0 && ScreenId.HasValue && ScreenId.Value > 0)
            {
                var existingScreen = await _screenService.GetScreenByIdAsync(ScreenId.Value);
                if (existingScreen != null)
                {
                    TheaterId = existingScreen.TheaterId;
                }
            }

            if (TheaterId > 0)
            {
                Theater = await _theaterService.GetTheaterByIdAsync(TheaterId);
            }

            Amenities = await _screenService.GetAsync();
            SeatCategories = await _screenService.GetSeatCategoriesAsync();

            if (ScreenId.HasValue && ScreenId.Value > 0)
            {
                var screen = await _screenService.GetScreenByIdAsync(ScreenId.Value);
                if (screen != null)
                {
                    ScreenSpecs = screen;
                }
                ExistingLayout = await _screenService.GetScreenLayoutAsync(ScreenId.Value);
            }

            return Page();
        }

        // ── Handler: Step 1 Save Details ─────────────────────────────────
        public async Task<IActionResult> OnPostSaveScreenDetailsAsync([FromBody] CreateScreen request)
        {
            if (request == null) return BadRequest("Invalid payload.");

            int userId = 0;
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

            if (request.TheaterId <= 0) request.TheaterId = TheaterId;

            // Prefer screenId from the JSON body (set by JS wizard state), then query param
            int resolvedScreenId = request.ScreenId > 0 ? request.ScreenId : (ScreenId ?? 0);

            if (resolvedScreenId > 0)
            {
                bool updated = await _screenService.UpdateScreenAsync(resolvedScreenId, request);
                return new JsonResult(new { success = updated, screenId = resolvedScreenId });
            }
            else
            {
                int newScreenId = await _screenService.CreateScreen(request, userId);
                return new JsonResult(new { success = newScreenId > 0, screenId = newScreenId });
            }
        }

        // ── Handler: Step 2 Save Layout ──────────────────────────────────
        public async Task<IActionResult> OnPostSaveScreenLayoutAsync([FromBody] ScreenLayoutDto request)
        {
            if (request == null || request.ScreenId <= 0) return BadRequest("Invalid layout payload.");

            var savedLayout = await _screenService.SaveScreenLayoutAsync(request);
            return new JsonResult(new { success = true, layout = savedLayout });
        }

        // ── Handler: Step 3 Save Seat Allocations ────────────────────────
        public async Task<IActionResult> OnPostSaveSeatAllocationAsync([FromBody] List<SeatAllocationDto> request)
        {
            if (request == null) return BadRequest("Invalid allocation payload.");

            bool result = await _screenService.SaveSeatAllocationAsync(request);
            return new JsonResult(new { success = result });
        }
    }
}
