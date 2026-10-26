using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBookingApplicationDTO_s;
using MovieTicketBookingInfrastructure.Service;

namespace MovieTicketBooking.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IAuthService _authService;

    public RegisterModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public Register Register { get; set; } = new();

    [BindProperty]
    public ResetPasswordRequest RequestPassword { get; set; } = new();

    public class ResetPasswordRequest
    {
        public string? Email { get; set; }
        public string? Otp { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult>
    OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        bool success = await _authService.RegisterAsync(Register);

        if (!success)
        {
            ModelState.AddModelError("", "Email already exists.");

            return Page();
        }

        if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
        {
            return RedirectToPage("/Account/Login", new { ReturnUrl });
        }

        return RedirectToPage("/Account/Login");
    }
}