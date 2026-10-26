using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.DTOs;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Users;

namespace MovieTicketBooking.Pages.SuperAdmin.Users
{
    [Authorize(Roles = "SuperAdmin")]
    public class UserListingModel : PageModel
    {
        private readonly IUserManagementService _userManagementService;

        public UserListingModel(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        public List<UserDto> UsersList { get; set; } = new();
        public List<RoleDto> Roles { get; set; } = new();

        [BindProperty]
        public CreateUserDto NewUser { get; set; } = new();

        public async Task OnGetAsync()
        {
            UsersList = await _userManagementService.GetUsersAsync();
            Roles = await _userManagementService.GetRolesAsync();
        }

        public async Task<IActionResult> OnPostCreateUserAjaxAsync()
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
                bool result = await _userManagementService.CreateUserAsync(NewUser);
                if (!result)
                {
                    return new JsonResult(new { success = false, errors = new Dictionary<string, string[]> { { string.Empty, new[] { "Unable to create User. Ensure email is unique." } } } });
                }
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, errors = new Dictionary<string, string[]> { { string.Empty, new[] { ex.Message } } } });
            }
        }

        public async Task<IActionResult> OnPostUpdateUserAjaxAsync()
        {
            // Remove validation for Password might be empty for updates if not changing
            ModelState.Remove(nameof(NewUser.Password));

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
                bool result = await _userManagementService.UpdateUserAsync(NewUser);
                if (!result)
                {
                    return new JsonResult(new { success = false, errors = new Dictionary<string, string[]> { { string.Empty, new[] { "Unable to update User." } } } });
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
