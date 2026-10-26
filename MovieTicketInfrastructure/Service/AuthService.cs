using BCrypt.Net;
using Microsoft.Win32;
using MovieTicketBooking.Application.DTOs;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBooking.Domain.Entities;
using MovieTicketBooking.Infrastructure.Repositories;
using MovieTicketBooking.Infrastructure.Services;
using MovieTicketBookingApplication.Interfaces;
using MovieTicketBookingApplicationDTO_s;
using MovieTicketBookingDomain.Entities;
using LoginRequest = MovieTicketBookingApplicationDTO_s.LoginRequest;


namespace MovieTicketBookingInfrastructure.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly IEmailService _emailService;
        private readonly OtpGenerator _otpGenerator;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            IAuthRepository authRepository,
            IPasswordResetRepository passwordResetRepository,
            IEmailService emailService,
            OtpGenerator otpGenerator,
            IJwtTokenService jwtTokenService)
        {
            _authRepository = authRepository;
            _passwordResetRepository = passwordResetRepository;
            _emailService = emailService;
            _otpGenerator = otpGenerator;
            _jwtTokenService = jwtTokenService;
        }

        #region Register

        public async Task<bool> RegisterAsync(Register request)
        {
            var existingUser = await _authRepository
                .GetUserByEmailAsync(request.Email);

            if (existingUser != null)
            {
                return false;
            }

            var passwordHash = BCrypt.Net.BCrypt
                .HashPassword(request.Password);

            var user = new Users
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                RoleId = RoleConstants.Customer
            };

            return await _authRepository
                .CreateUserAsync(user);
        }

        #endregion

        #region Login

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Step 1 : Find user
            var loginUser = await _authRepository
                .GetUserByEmailAsync(request.Email);

            if (loginUser == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid Email or Password."
                };
            }

            // Step 2 : Verify Password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(
                request.Password,
                loginUser.User.PasswordHash);

            if (!isPasswordValid)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid Email or Password."
                };
            }

            // Step 3 : Generate JWT
            string token = _jwtTokenService.GenerateToken(
                loginUser.User,
                loginUser.RoleName);

            // Step 4 : Return Login Response
            return new LoginResponse
            {
                Success = true,
                Token = token,
                User = loginUser.User,
                RoleName = loginUser.RoleName,
                Message = "Login Successful"
            };
        }

        #endregion





        public async Task ForgotPasswordAsync(ForgotPassword request)
        {
            // Step 1 : Check whether user exists
            var user = await _authRepository
                .GetUserByEmailAsync(request.Email);

            // Security:
            // Never reveal whether the email exists.
            if (user == null)
            {
                return;
            }

            // Step 2 : Invalidate all previous OTPs
            await _passwordResetRepository
                .InvalidatePreviousOtpsAsync(user.User.Id);

            // Step 3 : Generate OTP
            string otp = _otpGenerator.GenerateOtp();

            // Step 4 : Hash OTP
            string otpHash = BCrypt.Net.BCrypt
                .HashPassword(otp);

            // Step 5 : Create Entity
            PasswordResetOtp passwordResetOtp = new PasswordResetOtp
            {
                UserId = user.User.Id,
                OtpHash = otpHash,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            // Step 6 : Save OTP
            await _passwordResetRepository
                .CreateOtpAsync(passwordResetOtp);

            // Step 7 : Build Email
            string body = $@"
                <h2>Movie Ticket Booking</h2>

                <p>Hello {user.User.Username},</p>

                <p>Your Password Reset OTP is:</p>

                <h1 style='color:blue'>{otp}</h1>

                <p>This OTP will expire in <b>5 minutes</b>.</p>

                <p>Please do not share this OTP with anyone.</p>

                <br/>

                <p>Regards,</p>

                <b>Movie Ticket Booking Team</b>";

            // Step 8 : Send Email
            await _emailService.SendEmailAsync(
                user.User.Email,
                "Password Reset OTP",
                body);
        }

        public async Task<bool> VerifyOtpAsync(VerifyOtpRequest request)
        {
            // Step 1
            var user = await _authRepository
                .GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                return false;
            }

            // Step 2
            var otp = await _passwordResetRepository
                .GetLatestOtpAsync(user.User.Id);

            if (otp == null)
            {
                return false;
            }

            // Step 3
            if (otp.IsUsed)
            {
                return false;
            }

            // Step 4
            if (otp.ExpiryTime < DateTime.UtcNow)
            {
                return false;
            }

            // Step 5
            bool valid = BCrypt.Net.BCrypt.Verify(
                request.Otp,
                otp.OtpHash);

            return valid;
        }

        public async Task<bool> RequestPass(RequestPass request)
        {
            // Step 1 : Find User
            var user = await _authRepository
                .GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                return false;
            }

            // Step 2 : Get Latest OTP
            var otp = await _passwordResetRepository
                .GetLatestOtpAsync(user.User.Id);

            if (otp == null)
            {
                return false;
            }

            // Step 3 : OTP Already Used?
            if (otp.IsUsed)
            {
                return false;
            }

            // Step 4 : OTP Expired?
            if (otp.ExpiryTime < DateTime.UtcNow)
            {
                return false;
            }

            // Step 5 : Verify OTP
            bool validOtp = BCrypt.Net.BCrypt.Verify(
                request.Otp,
                otp.OtpHash);

            if (!validOtp)
            {
                return false;
            }

            // Step 6 : Password Confirmation
            if (request.NewPassword != request.ConfirmPassword)
            {
                return false;
            }

            // Check whether the new password is the same as the current password
            bool isSamePassword = BCrypt.Net.BCrypt.Verify(
                request.NewPassword,
                user.User.PasswordHash);

            if (isSamePassword)
            {
                return false;
            }

            // Step 7 : Hash New Password
            string passwordHash = BCrypt.Net.BCrypt
                .HashPassword(request.NewPassword);

            // Step 8 : Update Password
            bool updated = await _authRepository
                .UpdatePasswordAsync(user.User.Id, passwordHash);

            if (!updated)
            {
                return false;
            }

            // Step 9 : Mark OTP Used
            await _passwordResetRepository
                .MarkOtpAsUsedAsync(otp.Id);

            return true;
        }

    }
}