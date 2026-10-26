using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.Application.DTOs;


public class RequestPass
{
    public string Email { get; set; } = string.Empty;

    public string Otp { get; set; } = string.Empty;

    public string NewPassword { get; set; } = string.Empty;

    public string ConfirmPassword { get; set; } = string.Empty;
}
