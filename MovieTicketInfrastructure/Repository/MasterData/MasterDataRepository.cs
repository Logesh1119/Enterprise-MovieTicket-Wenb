using Dapper;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBookingDomain.Entities.MasterData;
using MovieTicketBookingDomain.Entities.Screen;
using MovieTicketBookingInfrastructureData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieTicketBooking.Infrastructure.Repositories
{
    public class MasterDataRepository : IMasterDataRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public MasterDataRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // Genres
        public async Task<List<Genres>> GetGenresAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return (await connection.QueryAsync<Genres>("SELECT * FROM Genres ORDER BY GenreName")).AsList();
        }
        public async Task<bool> CreateGenreAsync(Genres genre)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync("INSERT INTO Genres (GenreName) VALUES (@GenreName)", genre) > 0;
        }
        public async Task<bool> UpdateGenreAsync(Genres genre)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync("UPDATE Genres SET GenreName = @GenreName WHERE Id = @Id", genre) > 0;
        }
        public async Task<bool> DeleteGenreAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync("DELETE FROM Genres WHERE Id = @Id", new { Id = id }) > 0;
        }
        public async Task<bool> IsGenreReferencedAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM MovieGenres WHERE GenreId = @Id", new { Id = id }) > 0;
        }

        // SeatCategories
        public async Task<List<SeatCategories>> GetSeatCategoriesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return (await connection.QueryAsync<SeatCategories>("SELECT * FROM SeatCategories ORDER BY CategoryName")).AsList();
        }
        public async Task<bool> CreateSeatCategoryAsync(SeatCategories category)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync("INSERT INTO SeatCategories (CategoryName, ColorCode) VALUES (@CategoryName, @ColorCode)", category) > 0;
        }
        public async Task<bool> UpdateSeatCategoryAsync(SeatCategories category)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync("UPDATE SeatCategories SET CategoryName = @CategoryName, ColorCode = @ColorCode WHERE Id = @Id", category) > 0;
        }
        public async Task<bool> DeleteSeatCategoryAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync("DELETE FROM SeatCategories WHERE Id = @Id", new { Id = id }) > 0;
        }
        public async Task<bool> IsSeatCategoryReferencedAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            int screenRows = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM ScreenRows WHERE SeatCategoryId = @Id", new { Id = id });
            int showPrices = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM ShowSeatPrices WHERE SeatCategoryId = @Id", new { Id = id });
            return (screenRows > 0 || showPrices > 0);
        }

        // Amenities
        public async Task<List<AmenityMaster>> GetAmenitiesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return (await connection.QueryAsync<AmenityMaster>("SELECT Id, AmenityName FROM Amenities ORDER BY AmenityName")).AsList();
        }
        public async Task<bool> CreateAmenityAsync(AmenityMaster amenity)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync("INSERT INTO Amenities (AmenityName) VALUES (@AmenityName)", amenity) > 0;
        }
        public async Task<bool> UpdateAmenityAsync(AmenityMaster amenity)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync("UPDATE Amenities SET AmenityName = @AmenityName WHERE Id = @Id", amenity) > 0;
        }
        public async Task<bool> DeleteAmenityAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync("DELETE FROM Amenities WHERE Id = @Id", new { Id = id }) > 0;
        }
        public async Task<bool> IsAmenityReferencedAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM ScreenAmenities WHERE AmenityId = @Id", new { Id = id }) > 0;
        }

        // Certificates
        public async Task<List<Certificates>> GetCertificatesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return (await connection.QueryAsync<Certificates>("SELECT * FROM Certificates ORDER BY CertificateName")).AsList();
        }
        public async Task<bool> CreateCertificateAsync(Certificates certificate)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync("INSERT INTO Certificates (CertificateName) VALUES (@CertificateName)", certificate) > 0;
        }
        public async Task<bool> UpdateCertificateAsync(Certificates certificate)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync("UPDATE Certificates SET CertificateName = @CertificateName WHERE Id = @Id", certificate) > 0;
        }
        public async Task<bool> DeleteCertificateAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync("DELETE FROM Certificates WHERE Id = @Id", new { Id = id }) > 0;
        }
        public async Task<bool> IsCertificateReferencedAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM MoviesDb WHERE CertificateId = @Id", new { Id = id }) > 0;
        }
    }
}
