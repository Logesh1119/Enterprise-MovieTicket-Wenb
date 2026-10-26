using Dapper;
using MovieTicketBooking.Application.DTOs;
using MovieTicketBookingApplication.Interfaces;
using MovieTicketBookingDomain.Entities;
using MovieTicketBookingInfrastructureData;



namespace MovieTicketBookingInfrastructure.Service
{
    public class AuthRepository : IAuthRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public AuthRepository(
            SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<LoginUserDto?> GetUserByEmailAsync(string email)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            const string sql = "SELECT u.Id,u.Username,u.Email, u.PasswordHash, u.RoleId, r.RoleName AS RoleName FROM Users u INNER JOIN Roles r ON u.RoleId = r.RoleId WHERE u.Email = @Email";
            var data = await connection.QueryAsync(sql, new { Email = email });

            var row = data.FirstOrDefault();

            if (row == null)
                return null;

            return new LoginUserDto
            {
                User = new Users
                {
                    Id = row.Id,
                    Username = row.Username,
                    Email = row.Email,
                    PasswordHash = row.PasswordHash,
                    RoleId = row.RoleId
                },
                RoleName = row.RoleName
            };
        }

        public async Task<bool> CreateUserAsync(Users user)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            const string sql = """
            INSERT INTO Users
            (
                Username,
                Email,
                PasswordHash,
                RoleId
            )
            VALUES
            (
                @Username,
                @Email,
                @PasswordHash,
                @RoleId
            )
            """;

            var rowsAffected = await connection.ExecuteAsync(
                sql,
                user
            );

            return rowsAffected > 0;
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string passwordHash)
        {
            const string sql = @"
        UPDATE Users
        SET PasswordHash = @PasswordHash
        WHERE Id = @UserId";

            using var connection = _connectionFactory.CreateConnection();

            var rows = await connection.ExecuteAsync(sql, new
            {
                UserId = userId,
                PasswordHash = passwordHash
            });

            return rows > 0;
        }


    }
}


