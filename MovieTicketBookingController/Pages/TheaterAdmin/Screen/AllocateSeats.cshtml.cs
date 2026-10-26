using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen;
using MovieTicketBookingApplication.Interfaces.Screen;
using MovieTicketBookingDomain.Entities.Screen;

namespace MovieTicketBooking.Pages.TheaterAdmin.Screen
{
    [IgnoreAntiforgeryToken(Order = 1000)]
    public class AllocateSeatsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int ScreenId { get; set; }

        public ScreenLayoutDto? Layout { get; set; }

        public List<SeatCategories> Categories { get; set; } = new();

        private readonly IScreenService _screenService;

        public AllocateSeatsModel(IScreenService screenService)
        {
            _screenService = screenService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Layout = await _screenService.GetScreenLayoutAsync(ScreenId);

            if (Layout == null)
                return RedirectToPage("./ScreenCreation");

            Categories = await _screenService.GetSeatCategoriesAsync();

            return Page();
        }


        public async Task<IActionResult> OnPostSaveAllocationAsync(
            [FromBody] List<SeatAllocationDto> request)
        {
            if (request == null || !request.Any())
                return BadRequest();

            bool result =
                await _screenService.SaveSeatAllocationAsync(request);

            return new JsonResult(new
            {
                success = result
            });
        }
    }
}
