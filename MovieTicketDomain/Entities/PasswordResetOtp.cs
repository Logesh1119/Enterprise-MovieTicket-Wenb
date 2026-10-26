

using MovieTicketBookingDomain.Entities;

namespace MovieTicketBooking.Domain.Entities;

public class PasswordResetOtp
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string OtpHash { get; set; } = string.Empty;

    public DateTime ExpiryTime { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }

}