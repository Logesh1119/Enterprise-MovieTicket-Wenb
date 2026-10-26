using System.Security.Cryptography;

namespace MovieTicketBooking.Infrastructure.Services;

public class OtpGenerator
{
    public string GenerateOtp()
    {
        return RandomNumberGenerator
            .GetInt32(100000, 1000000)
            .ToString();
    }
}