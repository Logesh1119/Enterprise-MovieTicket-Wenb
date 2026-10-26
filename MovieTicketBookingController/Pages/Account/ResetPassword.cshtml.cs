using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.DTOs;
using MovieTicketBookingApplicationDTO_s;
using MovieTicketBookingInfrastructure.Service;

namespace MovieTicketBooking.Pages.Account;

public class ResetPasswordModel : PageModel
{
    private readonly IAuthService _authService;

    public ResetPasswordModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public RequestPass RequestPassword { get; set; } = new();

    public void OnGet(string email, string otp)
    {
        RequestPassword.Email = email;
        RequestPassword.Otp = otp;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        bool success = await _authService.RequestPass(RequestPassword);

        if (!success)
        {
            ModelState.AddModelError("", "Password Reset Failed");
            return Page();
        }

        TempData["Success"] = "Password Changed Successfully.";

        return RedirectToPage("Login");
    }
}