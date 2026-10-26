using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.DTOs;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Users;

namespace MovieTicketBooking.Pages.SuperAdmin.TheaterAdmins
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminListingModel : PageModel
    {
        private readonly IUserManagementService _userManagementService;

        public AdminListingModel(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        public List<UserDto> TheaterAdmins { get; set; } = new();

        [BindProperty]
        public CreateTheaterAdminRequest NewAdmin { get; set; } = new();

        [BindProperty]
        public CreateUserDto EditAdmin { get; set; } = new();

        public async Task OnGetAsync()
        {
            var allUsers = await _userManagementService.GetUsersAsync();
            TheaterAdmins = allUsers.Where(u => u.RoleName == "TheaterAdmin").ToList();
        }

        public async Task<IActionResult> OnPostCreateAdminAjaxAsync()
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
                bool result = await _userManagementService.CreateTheaterAdminAsync(NewAdmin);
                if (!result)
                {
                    return new JsonResult(new { success = false, errors = new Dictionary<string, string[]> { { string.Empty, new[] { "Unable to create Theater Admin. Ensure email is unique and passwords match." } } } });
                }
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, errors = new Dictionary<string, string[]> { { string.Empty, new[] { ex.Message } } } });
            }
        }

        public async Task<IActionResult> OnPostUpdateAdminAjaxAsync()
        {
            // Remove validation for NewAdmin since we're updating
            ModelState.Remove(nameof(NewAdmin.Username));
            ModelState.Remove(nameof(NewAdmin.Email));
            ModelState.Remove(nameof(NewAdmin.PhoneNumber));
            ModelState.Remove(nameof(NewAdmin.Country));
            ModelState.Remove(nameof(NewAdmin.Password));
            ModelState.Remove(nameof(NewAdmin.ConfirmPassword));
            
            // Password might be empty for updates if not changing
            ModelState.Remove(nameof(EditAdmin.Password));

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
                // Assign role id for theater admin.
                var roles = await _userManagementService.GetRolesAsync();
                EditAdmin.RoleId = roles.First(r => r.RoleName == "TheaterAdmin").RoleId;
                
                bool result = await _userManagementService.UpdateUserAsync(EditAdmin);
                if (!result)
                {
                    return new JsonResult(new { success = false, errors = new Dictionary<string, string[]> { { string.Empty, new[] { "Unable to update Theater Admin." } } } });
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
