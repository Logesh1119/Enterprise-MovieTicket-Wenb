using MovieTicketBooking.Domain.Entities;

namespace MovieTicketBooking.Application.Interfaces;

public interface IPasswordResetRepository
{
    Task CreateOtpAsync(PasswordResetOtp otp);

    Task<PasswordResetOtp?> GetLatestOtpAsync(int userId);

    Task MarkOtpAsUsedAsync(int otpId);

    Task InvalidatePreviousOtpsAsync(int userId);
}