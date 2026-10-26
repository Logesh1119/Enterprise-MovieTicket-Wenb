using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.DTOs;
using MovieTicketBookingApplicationDTO_s;
using MovieTicketBookingInfrastructure.Service;

namespace MovieTicketBooking.Pages.Account;

public class VerifyOtpModel : PageModel
{
    private readonly IAuthService _authService;

    public VerifyOtpModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public VerifyOtpRequest VerifyOtpRequest { get; set; } = new();

    public void OnGet(string email)
    {
        VerifyOtpRequest.Email = email;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        bool valid = await _authService.VerifyOtpAsync(VerifyOtpRequest);

        if (!valid)
        {
            ModelState.AddModelError("", "Invalid or Expired OTP");
            return Page();
        }

        return RedirectToPage(
            "ResetPassword",
            new
            {
                email = VerifyOtpRequest.Email,
                otp = VerifyOtpRequest.Otp
            });
    }
}