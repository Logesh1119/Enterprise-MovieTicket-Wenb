using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.Application.DTOs;

public class VerifyOtpRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Otp { get; set; } = string.Empty;
}