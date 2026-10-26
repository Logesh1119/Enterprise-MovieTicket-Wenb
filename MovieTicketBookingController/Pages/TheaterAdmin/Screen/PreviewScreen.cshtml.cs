using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen;
using MovieTicketBookingApplication.Interfaces;
using MovieTicketBookingApplication.Interfaces.Screen;
using MovieTicketBookingDomain.Entities.Screen;
using MovieTicketBookingDomain.Entities.Screens;
using NLog.Layouts;

namespace MovieTicketBooking.Pages.TheaterAdmin.Screen
{
    public class PreviewScreenModel : PageModel
    {
        private readonly IScreenService _screenService;

        public PreviewScreenModel(IScreenService screenService)
        {
            _screenService = screenService;
        }



        [BindProperty(SupportsGet = true)]
        public int ScreenId { get; set; }

        public ScreenLayoutDto? Layout { get; set; }

        public List<SeatCategories> Categories { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Layout = await _screenService.GetScreenLayoutAsync(ScreenId);

            if (Layout == null)
                return RedirectToPage("./ScreenCreation");

            Categories = await _screenService.GetSeatCategoriesAsync();

            return Page();
        }
    }
}