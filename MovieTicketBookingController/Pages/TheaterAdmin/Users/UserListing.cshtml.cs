using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieTicketBooking.Pages.TheaterAdmin.Users
{
    [IgnoreAntiforgeryToken(Order = 1000)]
    public class UserListingModel : PageModel
    {
        private readonly IUserManagementService _userService;

        public UserListingModel(IUserManagementService userService)
        {
            _userService = userService;
        }

        public List<RoleDto> Roles { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Roles = await _userService.GetRolesAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetUsersAjaxAsync()
        {
            var users = await _userService.GetUsersAsync();
            return new JsonResult(users);
        }

        public async Task<IActionResult> OnGetUserForEditAjaxAsync(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return new JsonResult(user);
        }

        public async Task<IActionResult> OnPostCreateUserAjaxAsync([FromBody] CreateUserDto request)
        {
            var result = await _userService.CreateUserAsync(request);
            return new JsonResult(new { success = result });
        }

        public async Task<IActionResult> OnPostUpdateUserAjaxAsync([FromBody] CreateUserDto request)
        {
            var result = await _userService.UpdateUserAsync(request);
            return new JsonResult(new { success = result });
        }

        public async Task<IActionResult> OnPostDeleteUserAjaxAsync(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return new JsonResult(new { success = result });
        }
    }
}
