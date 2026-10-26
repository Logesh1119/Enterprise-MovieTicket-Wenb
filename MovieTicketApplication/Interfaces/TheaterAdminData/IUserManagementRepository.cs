using MovieTicketBooking.Domain.Entities;
using MovieTicketBookingDomain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieTicketBooking.Application.Interfaces
{
    public interface IUserManagementRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task<bool> CreateTheaterAdminAsync(Users user, TheaterAdminProfile profile);
        
        Task<List<Users>> GetUsersAsync();
        Task<Users?> GetUserByIdAsync(int id);
        Task<TheaterAdminProfile?> GetProfileByUserIdAsync(int userId);
        
        Task<bool> CreateUserAsync(Users user, TheaterAdminProfile profile);
        Task<bool> UpdateUserAsync(Users user);
        Task<bool> UpdateProfileAsync(TheaterAdminProfile profile);
        Task<bool> DeleteUserAsync(int id);
        
        Task<List<Roles>> GetRolesAsync();
    }
}