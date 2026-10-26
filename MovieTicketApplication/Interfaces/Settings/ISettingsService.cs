using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Settings;
using System.Threading.Tasks;

namespace MovieTicketBooking.Application.Interfaces
{
    public interface ISettingsService
    {
        Task<ProfileDto?> GetProfileAsync(int userId);
        Task<bool> UpdateProfileAsync(ProfileDto profile);
        Task<bool> ChangePasswordAsync(ChangePasswordDto request);
    }
}
