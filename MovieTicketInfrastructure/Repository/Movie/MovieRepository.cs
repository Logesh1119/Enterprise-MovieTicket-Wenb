using Dapper;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Movies;
using MovieTicketBookingApplication.Interfaces.Movie;
using MovieTicketBookingDomain.Entities.Movies;
using MovieTicketBookingInfrastructureData;
using System.Data;

namespace MovieTicketBooking.Infrastructure.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public MovieRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateMovieAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        MoviesDb movie)
    {
        string sql = @"
        INSERT INTO MoviesDb
        (
            TheaterId,
            MovieName,
            MovieCode,
            MovieDescription,
            TrailerUrl,
            TeaserUrl,
            Duration,
            ReleaseDate,
            CertificateId,
            Status,
            Tags
        )
        VALUES
        (
            @TheaterId,
            @MovieName,
            @MovieCode,
            @MovieDescription,
            @TrailerUrl,
            @TeaserUrl,
            @Duration,
            @ReleaseDate,
            @CertificateId,
            @Status,
            @Tags
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);";

        return await connection.ExecuteScalarAsync<int>(
            sql,
            movie,
            transaction);
    }

    public async Task CreateMovieGenreAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        MovieGenres movieGenre)
    {
        string sql = @"
        INSERT INTO MovieGenres
        (
            MovieId,
            GenreId
        )
        VALUES
        (
            @MovieId,
            @GenreId
        );";

        await connection.ExecuteAsync(
            sql,
            movieGenre,
            transaction);
    }

    public async Task AddMovieImageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        MovieImages image)
    {
        string sql = @"
        INSERT INTO MovieImages
        (
            MovieId,
            MediaId,
            DisplayOrder,
            IsPrimary
        )
        VALUES
        (
            @MovieId,
            @MediaId,
            @DisplayOrder,
            @IsPrimary
        );";

        await connection.ExecuteAsync(
            sql,
            image,
            transaction);
    }


    public async Task<List<GenreDto>> GetGenresAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        string sql = @"
        SELECT
            Id,
            GenreName
        FROM Genres
        ORDER BY GenreName;";

        var result = await connection.QueryAsync<GenreDto>(sql);

        return result.ToList();
    }

    public async Task<List<CertificateDto>> GetCertificatesAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        string sql = @"
        SELECT
            Id,
            CertificateName
        FROM Certificates
        ORDER BY CertificateName;";

        var result = await connection.QueryAsync<CertificateDto>(sql);

        return result.ToList();
    }

    public async Task<int> GetTheaterIdByUserIdAsync(int? userId = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        string sql = @"
        SELECT Id
        FROM Theaters
        WHERE OwnerUserId = @UserId;";

        return await connection.ExecuteScalarAsync<int>(
            sql,
            new { UserId = userId });
    }

    public async Task<List<MovieCardDto>> GetMoviesAsync(int? userId = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        string sql = @"
SELECT
    m.Id AS MovieId,
    m.TheaterId,
    m.MovieName,
    m.MovieCode,
    m.Duration,
    m.ReleaseDate,
    m.Status,
    cert.CertificateName,
    img.FilePath AS ImagePath,
    (SELECT STRING_AGG(g.GenreName, ',') FROM MovieGenres mg JOIN Genres g ON g.Id = mg.GenreId WHERE mg.MovieId = m.Id) AS GenresString
FROM MoviesDb m
LEFT JOIN Certificates cert ON cert.Id = m.CertificateId
LEFT JOIN MovieImages mi ON mi.MovieId = m.Id AND mi.IsPrimary = 1
LEFT JOIN Media img ON img.Id = mi.MediaId
WHERE (@UserId IS NULL OR m.TheaterId IN (SELECT Id FROM Theaters WHERE OwnerUserId = @UserId))
ORDER BY m.MovieName;";

        var movies = (await connection.QueryAsync<MovieCardDto>(sql, new { UserId = userId })).ToList();

        foreach (var movie in movies)
        {
            movie.Genres = string.IsNullOrWhiteSpace(movie.GenresString)
                ? new List<string>()
                : movie.GenresString.Split(',', StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
        }

        return movies;
    }

    public async Task<CreateMovieRequest?> GetMovieByIdAsync(int movieId, int? userId = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        string sql = @"
        SELECT 
            m.Id AS MovieId,
            m.TheaterId,
            m.MovieName,
            m.MovieCode,
            m.MovieDescription,
            m.TrailerUrl,
            m.TeaserUrl,
            m.Duration,
            m.ReleaseDate,
            m.CertificateId,
            m.Status,
            m.Tags,
            (SELECT STRING_AGG(GenreId, ',') FROM MovieGenres WHERE MovieId = m.Id) AS GenreIds
        FROM MoviesDb m
        WHERE m.Id = @MovieId AND (@UserId IS NULL OR m.TheaterId IN (SELECT Id FROM Theaters WHERE OwnerUserId = @UserId));";

        var data = await connection.QuerySingleOrDefaultAsync(sql, new { MovieId = movieId, UserId = userId });

        if (data == null) return null;

        return new CreateMovieRequest
        {
            TheaterId = data.TheaterId,
            MovieName = data.MovieName,
            MovieCode = data.MovieCode,
            MovieDescription = data.MovieDescription,
            TrailerUrl = data.TrailerUrl ?? "",
            TeaserUrl = data.TeaserUrl ?? "",
            Duration = data.Duration,
            ReleaseDate = data.ReleaseDate,
            CertificateId = data.CertificateId,
            Status = data.Status,
            Tags = data.Tags ?? "",
            GenreId = string.IsNullOrWhiteSpace((string)data.GenreIds) 
                ? new List<int>() 
                : ((string)data.GenreIds).Split(',').Select(int.Parse).ToList()
        };
    }

    public async Task UpdateMovieAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        MoviesDb movie)
    {
        string sql = @"
        UPDATE MoviesDb SET
            MovieName = @MovieName,
            MovieCode = @MovieCode,
            MovieDescription = @MovieDescription,
            TrailerUrl = @TrailerUrl,
            TeaserUrl = @TeaserUrl,
            Duration = @Duration,
            ReleaseDate = @ReleaseDate,
            CertificateId = @CertificateId,
            Status = @Status,
            Tags = @Tags
        WHERE Id = @Id;";

        await connection.ExecuteAsync(sql, movie, transaction);
    }

    public async Task DeleteMovieGenresAsync(IDbConnection connection, IDbTransaction transaction, int movieId)
    {
        string sql = "DELETE FROM MovieGenres WHERE MovieId = @MovieId;";
        await connection.ExecuteAsync(sql, new { MovieId = movieId }, transaction);
    }
}