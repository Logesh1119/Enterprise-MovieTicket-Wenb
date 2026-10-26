using Microsoft.AspNetCore.Identity.Data;
using MovieTicketBooking.Application.DTOs;
using MovieTicketBookingApplicationDTO_s;
using MovieTicketBookingDomain.Entities;
using LoginRequest = MovieTicketBookingApplicationDTO_s.LoginRequest;

namespace MovieTicketBookingInfrastructure.Service
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(Register request);

        Task<LoginResponse> LoginAsync(LoginRequest request);

        Task ForgotPasswordAsync(ForgotPassword request);

        Task<bool> VerifyOtpAsync(VerifyOtpRequest request);

        Task<bool> RequestPass(RequestPass request);


    }
}