using MovieTicketBooking.Application.DTOs;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieTicketBooking.Application.Interfaces
{
    public interface IUserManagementService
    {
        Task<bool> CreateTheaterAdminAsync(CreateTheaterAdminRequest request);
        Task<List<UserDto>> GetUsersAsync();
        Task<CreateUserDto?> GetUserByIdAsync(int id);
        Task<bool> CreateUserAsync(CreateUserDto request);
        Task<bool> UpdateUserAsync(CreateUserDto request);
        Task<bool> DeleteUserAsync(int id);
        Task<List<RoleDto>> GetRolesAsync();
    }
}