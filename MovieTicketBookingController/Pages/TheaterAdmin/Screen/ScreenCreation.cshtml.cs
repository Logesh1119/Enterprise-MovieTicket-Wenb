using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen;
using MovieTicketBookingApplication.Interfaces.Screen;
using MovieTicketBookingDomain.Entities.Screens;
using System.Security.Claims;

namespace MovieTicketBooking.Pages.TheaterAdmin.Screen
{
    public class ScreenCreationModel : PageModel
    {
        private readonly IScreenService _screenService;

        public ScreenCreationModel(IScreenService screenService)
        {
            _screenService = screenService;
        }

        [BindProperty]
        public CreateScreen Screen { get; set; } = new();

        public List<CreateAmenity> Amenities { get; set; } = new();

        public List<string> ScreenTypes { get; set; } = new();

        public List<string> AudioTechnologies { get; set; } = new();

        public List<string> SeatingTypes { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadDropdowns();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return Page();
            }

            int userId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            int screenId = await _screenService.CreateScreen(
                Screen,
                userId);

            if (screenId <= 0)
            {
                ModelState.AddModelError("", "Unable to create screen.");

                await LoadDropdowns();

                return Page();
            }

            TempData["Success"] = "Screen created successfully.";

            return RedirectToPage(
    "/TheaterAdmin/Screen/ConfigureSeats",
    new { ScreenId = screenId });
        }

        private async Task LoadDropdowns()
        {
            Amenities = await _screenService.GetAsync();

            ScreenTypes = new()
            {
                "Standard",
                "IMAX",
                "Dolby Cinema",
                "4DX",
                "ScreenX"
            };

            AudioTechnologies = new()
            {
                "Dolby Atmos",
                "Dolby Digital 7.1",
                "DTS:X",
                "Auro 3D"
            };

            SeatingTypes = new()
            {
                "Standard",
                "Recliner",
                "Luxury Recliner"
            };
        }
    }
}