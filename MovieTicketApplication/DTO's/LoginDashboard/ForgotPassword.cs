using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingApplicationDTO_s;

public class ForgotPassword
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}