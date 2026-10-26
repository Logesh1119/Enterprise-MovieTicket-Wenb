using Dapper;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBooking.Domain.Entities;
using MovieTicketBookingDomain.Entities;
using MovieTicketBookingInfrastructureData;
using System.Threading.Tasks;

namespace MovieTicketBooking.Infrastructure.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public SettingsRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Users?> GetUserByIdAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Users>("SELECT * FROM Users WHERE Id = @Id", new { Id = userId });
        }

        public async Task<TheaterAdminProfile?> GetProfileByUserIdAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<TheaterAdminProfile>("SELECT * FROM TheaterAdminProfile WHERE UserId = @UserId", new { UserId = userId });
        }

        public async Task<string?> GetMediaPathAsync(int mediaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<string>("SELECT FilePath FROM Media WHERE Id = @Id", new { Id = mediaId });
        }

        public async Task<bool> UpdateUserAsync(Users user)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync("UPDATE Users SET Username = @Username, Email = @Email WHERE Id = @Id", user) > 0;
        }

        public async Task<bool> UpdateProfileAsync(TheaterAdminProfile profile)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync("UPDATE TheaterAdminProfile SET PhoneNumber = @PhoneNumber, Country = @Country WHERE UserId = @UserId", profile) > 0;
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string newPasswordHash)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync("UPDATE Users SET PasswordHash = @PasswordHash WHERE Id = @Id", new { PasswordHash = newPasswordHash, Id = userId }) > 0;
        }
    }
}
