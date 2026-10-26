using MovieTicketBooking.Domain.Entities;
using MovieTicketBookingDomain.Entities;
using System.Threading.Tasks;

namespace MovieTicketBooking.Application.Interfaces
{
    public interface ISettingsRepository
    {
        Task<Users?> GetUserByIdAsync(int userId);
        Task<TheaterAdminProfile?> GetProfileByUserIdAsync(int userId);
        Task<string?> GetMediaPathAsync(int mediaId);
        Task<bool> UpdateUserAsync(Users user);
        Task<bool> UpdateProfileAsync(TheaterAdminProfile profile);
        Task<bool> UpdatePasswordAsync(int userId, string newPasswordHash);
    }
}
