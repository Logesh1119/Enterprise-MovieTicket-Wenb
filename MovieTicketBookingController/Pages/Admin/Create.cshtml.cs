using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.DTOs;
using MovieTicketBooking.Application.Interfaces;

namespace MovieTicketBooking.Pages.Admin.TheaterAdmin
{
    public class CreateModel : PageModel
    {
        private readonly IUserManagementService _userManagementService;

        public CreateModel(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        [BindProperty]
        public CreateTheaterAdminRequest TheaterAdmin { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var item in ModelState)
                {
                    foreach (var error in item.Value.Errors)
                    {
                        Console.WriteLine($"{item.Key} : {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            bool result = await _userManagementService.CreateTheaterAdminAsync(TheaterAdmin);

            if (!result)
            {
                ModelState.AddModelError("", "Unable to create Theater Admin.");
                return Page();
            }

            TempData["Success"] = "Theater Admin created successfully.";

            return RedirectToPage("/TheaterAdmin/Theater/TheaterListing");
        }
    }
}