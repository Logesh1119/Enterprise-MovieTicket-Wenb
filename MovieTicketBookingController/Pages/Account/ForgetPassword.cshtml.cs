using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBookingApplicationDTO_s;
using MovieTicketBookingInfrastructure.Service;


namespace MovieTicketBooking.Pages.Account;

public class ForgotPasswordModel : PageModel
{
    private readonly IAuthService _authService;

    public ForgotPasswordModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public ForgotPassword ForgotPassword { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        await _authService
            .ForgotPasswordAsync(ForgotPassword);

        return RedirectToPage("/Account/VerifyPassword",
            new { email = ForgotPassword.Email });
    }
}