using Microsoft.AspNetCore.Identity.Data;
using MovieTicketBooking.Application.DTOs;
using MovieTicketBookingApplicationDTO_s;
using MovieTicketBookingDomain.Entities;

namespace MovieTicketBookingApplication.Interfaces
{
    public interface IAuthRepository
    {
        Task<LoginUserDto?> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(Users user);
        Task<bool> UpdatePasswordAsync(int userId, string passwordHash);
    }
}
