using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBookingApplicationDTO_s;
using MovieTicketBookingInfrastructure.Service;

namespace MovieTicketBooking.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IAuthService _authService;

    public LoginModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public LoginRequest Login { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var loginResult = await _authService.LoginAsync(Login);

        if (!loginResult.Success)
        {
            ModelState.AddModelError("", loginResult.Message);
            return Page();
        }

        Response.Cookies.Append(
            "MovieTicketBookingJWT",
            loginResult.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,              // Change to false only if you're running without HTTPS during development
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

        if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
        {
            return LocalRedirect(ReturnUrl);
        }

        if (loginResult.RoleName == "SuperAdmin")
        {
            return RedirectToPage("/Admin/Create");
        }

        if (loginResult.RoleName == "TheaterAdmin")
        {
            return RedirectToPage("/TheaterAdmin/Theater/TheaterListing");
        }

        return RedirectToPage("/Customer/Home/Index");
    }

    public IActionResult OnPostLogout()
    {
        Response.Cookies.Delete("MovieTicketBookingJWT");
        return RedirectToPage("/Customer/Home/Index");
    }
}