using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Settings;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieTicketBooking.Pages.SuperAdmin.Settings
{
    [IgnoreAntiforgeryToken(Order = 1000)]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SuperAdmin")]
    public class SettingsModel : PageModel
    {
        private readonly ISettingsService _service;

        public SettingsModel(ISettingsService service)
        {
            _service = service;
        }

        public int CurrentUserId { get; set; } = 1; // Default fallback
        public ProfileDto? Profile { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int uid)) CurrentUserId = uid;

            Profile = await _service.GetProfileAsync(CurrentUserId);
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateProfileAjaxAsync([FromBody] ProfileDto dto)
        {
            var success = await _service.UpdateProfileAsync(dto);
            return new JsonResult(new { success });
        }

        public async Task<IActionResult> OnPostChangePasswordAjaxAsync([FromBody] ChangePasswordDto dto)
        {
            var success = await _service.ChangePasswordAsync(dto);
            return new JsonResult(new { success });
        }
    }
}
