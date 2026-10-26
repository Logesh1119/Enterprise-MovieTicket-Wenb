using MovieTicketBooking.Application.Interfaces;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Settings;
using System.Threading.Tasks;

namespace MovieTicketBooking.Infrastructure.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsRepository _repository;

        public SettingsService(ISettingsRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProfileDto?> GetProfileAsync(int userId)
        {
            var user = await _repository.GetUserByIdAsync(userId);
            if (user == null) return null;

            var profile = await _repository.GetProfileByUserIdAsync(userId);
            string? imagePath = null;
            if (profile != null && profile.ProfileImageId.HasValue)
            {
                imagePath = await _repository.GetMediaPathAsync(profile.ProfileImageId.Value);
            }

            return new ProfileDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = profile?.PhoneNumber ?? "",
                Country = profile?.Country ?? "",
                ProfileImagePath = imagePath
            };
        }

        public async Task<bool> UpdateProfileAsync(ProfileDto profileDto)
        {
            var user = await _repository.GetUserByIdAsync(profileDto.UserId);
            if (user == null) return false;

            user.Username = profileDto.Username;
            user.Email = profileDto.Email;
            await _repository.UpdateUserAsync(user);

            var profile = await _repository.GetProfileByUserIdAsync(profileDto.UserId);
            if (profile != null)
            {
                profile.PhoneNumber = profileDto.PhoneNumber;
                profile.Country = profileDto.Country;
                await _repository.UpdateProfileAsync(profile);
            }

            return true;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto request)
        {
            var user = await _repository.GetUserByIdAsync(request.UserId);
            if (user == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                return false;

            var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            return await _repository.UpdatePasswordAsync(request.UserId, newHash);
        }
    }
}
