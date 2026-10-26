using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Theater;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.TheaterCreation;

namespace MovieTicketBooking.Pages.SuperAdmin.Theater
{
    [Authorize(Roles = "SuperAdmin")]
    public class TheaterListModel : PageModel
    {
        private readonly ITheaterService _theaterService;
        private readonly IUserManagementService _userManagementService;

        public TheaterListModel(ITheaterService theaterService, IUserManagementService userManagementService)
        {
            _theaterService = theaterService;
            _userManagementService = userManagementService;
        }

        public List<TheaterCardDto> Theaters { get; set; } = new();
        public SelectList TheaterAdminsList { get; set; } = default!;

        // ── Drawer form binding ──────────────────────────────────────────
        [BindProperty]
        public CreateTheater NewTheater { get; set; } = new();

        // ── GET ──────────────────────────────────────────────────────────
        public async Task OnGetAsync()
        {
            Theaters = await _theaterService.GetTheatersAsync();
            var users = await _userManagementService.GetUsersAsync();
            var theaterAdmins = users.Where(u => u.RoleName == "TheaterAdmin").ToList();
            TheaterAdminsList = new SelectList(theaterAdmins, "Id", "Username");
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