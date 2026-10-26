using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen;
using MovieTicketBookingApplication.Interfaces.Screen;
using NLog.Layouts;

namespace MovieTicketBooking.Pages.TheaterAdmin.Screen
{
    [IgnoreAntiforgeryToken(Order = 1000)]
    public class ConfigureSeatsModel : PageModel
    {
        private readonly IScreenService _screenService;

        public ConfigureSeatsModel(IScreenService screenService)
        {
            _screenService = screenService;
        }

        [BindProperty(SupportsGet = true)]
        public int ScreenId { get; set; }

        public ScreenLayoutDto Layout { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Layout = await _screenService.GetScreenLayoutAsync(ScreenId);

            if (Layout == null)
            {
                Layout = new ScreenLayoutDto
                {
                    ScreenId = ScreenId
                };
            }

            return Page();
        }


        public async Task<IActionResult> OnPostSaveLayoutAsync(
            [FromBody] ScreenLayoutDto request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Request is null.");

                var savedLayout = await _screenService.SaveScreenLayoutAsync(request);

                return new JsonResult(new
                {
                    success = true,
                    layout = savedLayout
                });
            }

            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
