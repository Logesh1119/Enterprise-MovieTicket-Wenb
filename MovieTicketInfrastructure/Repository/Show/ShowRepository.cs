using Dapper;
using MovieTicketBookingApplication.Interfaces.Show;
using MovieTicketBookingDomain.Entities.Shows;
using MovieTicketBookingInfrastructureData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MovieTicketBookingInfrastructure.Repository.Show
{
    public class ShowRepository : IShowRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public ShowRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateShowAsync(IDbConnection connection, IDbTransaction transaction, Shows show)
        {
            string sql = @"
                INSERT INTO Shows
                (ScreenId, MovieId, ShowDate, ShowTime, Language, Format, Status, CreatedAt, UpdatedAt)
                VALUES
                (@ScreenId, @MovieId, @ShowDate, @ShowTime, @Language, @Format, @Status, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            
            return await connection.QuerySingleAsync<int>(sql, show, transaction);
        }

        public async Task UpdateShowAsync(IDbConnection connection, IDbTransaction transaction, Shows show)
        {
            string sql = @"
                UPDATE Shows
                SET ScreenId = @ScreenId,
                    MovieId = @MovieId,
                    ShowDate = @ShowDate,
                    ShowTime = @ShowTime,
                    Language = @Language,
                    Format = @Format,
                    Status = @Status,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id;";
            
            await connection.ExecuteAsync(sql, show, transaction);
        }

        public async Task DeleteShowAsync(IDbConnection connection, IDbTransaction transaction, int showId)
        {
            string sql = "DELETE FROM Shows WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, new { Id = showId }, transaction);
        }

        public async Task CreateShowSeatPriceAsync(IDbConnection connection, IDbTransaction transaction, ShowSeatPrices price)
        {
            string sql = @"
                INSERT INTO ShowSeatPrices
                (ShowId, SeatCategoryId, Price, CreatedAt, UpdatedAt)
                VALUES
                (@ShowId, @SeatCategoryId, @Price, @CreatedAt, @UpdatedAt);";
            
            await connection.ExecuteAsync(sql, price, transaction);
        }

        public async Task DeleteShowSeatPricesAsync(IDbConnection connection, IDbTransaction transaction, int showId)
        {
            string sql = "DELETE FROM ShowSeatPrices WHERE ShowId = @ShowId;";
            await connection.ExecuteAsync(sql, new { ShowId = showId }, transaction);
        }

        public async Task<IEnumerable<Shows>> GetShowsByScreenAndDateAsync(int screenId, DateTime date)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = "SELECT * FROM Shows WHERE ScreenId = @ScreenId AND CAST(ShowDate AS DATE) = CAST(@ShowDate AS DATE);";
            return await connection.QueryAsync<Shows>(sql, new { ScreenId = screenId, ShowDate = date });
        }

        public async Task<Shows?> GetShowByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = "SELECT * FROM Shows WHERE Id = @Id;";
            return await connection.QuerySingleOrDefaultAsync<Shows>(sql, new { Id = id });
        }

        public async Task<IEnumerable<ShowSeatPrices>> GetShowSeatPricesAsync(int showId)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = "SELECT * FROM ShowSeatPrices WHERE ShowId = @ShowId;";
            return await connection.QueryAsync<ShowSeatPrices>(sql, new { ShowId = showId });
        }
    }
}
