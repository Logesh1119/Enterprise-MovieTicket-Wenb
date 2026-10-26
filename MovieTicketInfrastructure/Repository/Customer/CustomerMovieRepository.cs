using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using MovieTicketBooking.Application.DTOs.Customer.Movie;
using MovieTicketBooking.Application.Interfaces.Customer;

namespace MovieTicketBooking.Infrastructure.Repository.Customer
{
    public class CustomerMovieRepository : ICustomerMovieRepository
    {
        private readonly string _connectionString;

        public CustomerMovieRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<CustomerMovieDto>> GetFeaturedMoviesAsync()
        {
            using var connection = CreateConnection();
            string query = @"
                SELECT 
                    MIN(m.Id) as Id,
                    m.MovieName,
                    MIN(m.Duration) as Duration,
                    MIN(m.ReleaseDate) as ReleaseDate,
                    MIN(c.CertificateName) as CertificateName,
                    MIN(med.FilePath) as PosterUrl
                FROM MoviesDb m
                LEFT JOIN Certificates c ON m.CertificateId = c.Id
                LEFT JOIN MovieImages mi ON m.Id = mi.MovieId AND mi.IsPrimary = 1
                LEFT JOIN Media med ON mi.MediaId = med.Id
                WHERE m.Status = 1
                GROUP BY m.MovieName
                ORDER BY MIN(m.Id) DESC";
            
            return await connection.QueryAsync<CustomerMovieDto>(query);
        }

        public async Task<IEnumerable<CustomerMovieDto>> GetUpcomingMoviesAsync()
        {
            using var connection = CreateConnection();
            string query = @"
                SELECT 
                    MIN(m.Id) as Id,
                    m.MovieName,
                    MIN(m.Duration) as Duration,
                    MIN(m.ReleaseDate) as ReleaseDate,
                    MIN(c.CertificateName) as CertificateName,
                    MIN(med.FilePath) as PosterUrl
                FROM MoviesDb m
                LEFT JOIN Certificates c ON m.CertificateId = c.Id
                LEFT JOIN MovieImages mi ON m.Id = mi.MovieId AND mi.IsPrimary = 1
                LEFT JOIN Media med ON mi.MediaId = med.Id
                WHERE m.Status = 0
                GROUP BY m.MovieName
                ORDER BY MIN(m.Id) DESC";
            
            return await connection.QueryAsync<CustomerMovieDto>(query);
        }

        public async Task<IEnumerable<CustomerMovieDto>> GetAllMoviesAsync(string search, string genre)
        {
            using var connection = CreateConnection();
            string query = @"
                SELECT 
                    MIN(m.Id) as Id,
                    m.MovieName,
                    MIN(m.Duration) as Duration,
                    MIN(m.ReleaseDate) as ReleaseDate,
                    MIN(c.CertificateName) as CertificateName,
                    MIN(med.FilePath) as PosterUrl
                FROM MoviesDb m
                LEFT JOIN Certificates c ON m.CertificateId = c.Id
                LEFT JOIN MovieImages mi ON m.Id = mi.MovieId AND mi.IsPrimary = 1
                LEFT JOIN Media med ON mi.MediaId = med.Id
                WHERE 1=1 ";
            
            if (!string.IsNullOrEmpty(search))
            {
                query += " AND m.MovieName LIKE @Search ";
            }
            
            // To filter by genre properly we should join MovieGenres but since we group by MovieName,
            // we can just use an IN clause or exists.
            if (!string.IsNullOrEmpty(genre))
            {
                query += @" AND EXISTS (
                    SELECT 1 FROM MovieGenres mg 
                    INNER JOIN Genres g ON mg.GenreId = g.Id
                    WHERE mg.MovieId = m.Id AND g.GenreName = @Genre
                )";
            }

            query += @" GROUP BY m.MovieName ORDER BY MIN(m.Id) DESC";

            return await connection.QueryAsync<CustomerMovieDto>(query, new { Search = $"%{search}%", Genre = genre });
        }

        public async Task<CustomerMovieDetailDto> GetMovieByIdAsync(int id)
        {
            using var connection = CreateConnection();
            // We get the movie name first, to fetch all details.
            // Since multiple theaters can have the same movie name, we just get the details from this specific ID.
            string query = @"
                SELECT 
                    m.Id,
                    m.MovieName,
                    m.MovieDescription,
                    m.TrailerUrl,
                    m.Duration,
                    m.ReleaseDate,
                    c.CertificateName,
                    m.Tags,
                    (SELECT TOP 1 med.FilePath FROM MovieImages mi INNER JOIN Media med ON mi.MediaId = med.Id WHERE mi.MovieId = m.Id AND mi.IsPrimary = 1) as PosterUrl,
                    (SELECT TOP 1 med.FilePath FROM MovieImages mi INNER JOIN Media med ON mi.MediaId = med.Id WHERE mi.MovieId = m.Id AND mi.IsPrimary = 0) as BannerUrl
                FROM MoviesDb m
                LEFT JOIN Certificates c ON m.CertificateId = c.Id
                WHERE m.Id = @Id";

            var movie = await connection.QueryFirstOrDefaultAsync<CustomerMovieDetailDto>(query, new { Id = id });
            
            if (movie != null)
            {
                // Fetch genres
                string genreQuery = @"
                    SELECT g.GenreName 
                    FROM MovieGenres mg
                    INNER JOIN Genres g ON mg.GenreId = g.Id
                    WHERE mg.MovieId = @Id";
                var genres = await connection.QueryAsync<string>(genreQuery, new { Id = id });
                movie.Genres = string.Join("", "", genres);
            }

            return movie;
        }
    }
}
