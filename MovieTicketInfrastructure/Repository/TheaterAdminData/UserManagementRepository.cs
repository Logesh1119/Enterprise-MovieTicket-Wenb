using Dapper;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBooking.Domain.Entities;
using MovieTicketBookingDomain.Entities;
using MovieTicketBookingInfrastructureData;

namespace MovieTicketBooking.Infrastructure.Repositories
{
    public class UserManagementRepository : IUserManagementRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public UserManagementRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                SELECT COUNT(1)
                FROM Users
                WHERE Email = @Email";

            int count = await connection.ExecuteScalarAsync<int>(
                sql,
                new { Email = email });

            return count > 0;
        }

        public async Task<bool> CreateTheaterAdminAsync(
            Users user, TheaterAdminProfile profile)
        {
            using var connection = _connectionFactory.CreateConnection();

            // IDbConnection requires synchronous Open()
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                const string userSql = @"
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
                    );

                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                int userId = await connection.ExecuteScalarAsync<int>(
                    userSql,
                    user,
                    transaction);

                profile.UserId = userId;

                const string profileSql = @"
                    INSERT INTO TheaterAdminProfile
                    (
                        UserId,
                        ProfileImageId,
                        PhoneNumber,
                        Country,
                        Status,
                        CreatedAt
                    )
                    VALUES
                    (
                        @UserId,
                        @ProfileImageId,
                        @PhoneNumber,
                        @Country,
                        @Status,
                        @CreatedAt
                    );";

                int rowsAffected = await connection.ExecuteAsync(
                    profileSql,
                    profile,
                    transaction);

                if (rowsAffected <= 0)
                {
                    transaction.Rollback();
                    return false;
                }

                transaction.Commit();

                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
        public async Task<List<Users>> GetUsersAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT u.Id, u.Username, u.Email, u.RoleId, r.RoleName
                FROM Users u
                LEFT JOIN Roles r ON u.RoleId = r.RoleId
                ORDER BY u.Id DESC";
            return (await connection.QueryAsync<Users>(sql)).AsList();
        }

        public async Task<Users?> GetUserByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM Users WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Users>(sql, new { Id = id });
        }

        public async Task<TheaterAdminProfile?> GetProfileByUserIdAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM TheaterAdminProfile WHERE UserId = @UserId";
            return await connection.QueryFirstOrDefaultAsync<TheaterAdminProfile>(sql, new { UserId = userId });
        }

        public async Task<bool> CreateUserAsync(Users user, TheaterAdminProfile profile)
        {
            return await CreateTheaterAdminAsync(user, profile);
        }

        public async Task<bool> UpdateUserAsync(Users user)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE Users 
                SET Username = @Username, Email = @Email, RoleId = @RoleId
                WHERE Id = @Id";
            int rows = await connection.ExecuteAsync(sql, user);
            return rows > 0;
        }

        public async Task<bool> UpdateProfileAsync(TheaterAdminProfile profile)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE TheaterAdminProfile 
                SET PhoneNumber = @PhoneNumber, Country = @Country, Status = @Status
                WHERE UserId = @UserId";
            int rows = await connection.ExecuteAsync(sql, profile);
            return rows > 0;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                await connection.ExecuteAsync("DELETE FROM TheaterAdminProfile WHERE UserId = @Id", new { Id = id }, transaction);
                int rows = await connection.ExecuteAsync("DELETE FROM Users WHERE Id = @Id", new { Id = id }, transaction);
                transaction.Commit();
                return rows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<List<Roles>> GetRolesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return (await connection.QueryAsync<Roles>("SELECT RoleId, RoleName FROM Roles ORDER BY RoleName")).AsList();
        }
    }
}