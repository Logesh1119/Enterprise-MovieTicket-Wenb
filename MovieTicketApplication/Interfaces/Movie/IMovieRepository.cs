using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Movies;
using MovieTicketBookingDomain.Entities.Movies;
using System.Data;

namespace MovieTicketBookingApplication.Interfaces.Movie
{
    public interface IMovieRepository
    {
        Task<int> CreateMovieAsync(IDbConnection connection,
    IDbTransaction transaction, MoviesDb movies);

        Task AddMovieImageAsync(IDbConnection connection,
            IDbTransaction transaction, MovieImages image);

        Task CreateMovieGenreAsync(IDbConnection connection, IDbTransaction transaction, MovieGenres movieGenre);

        Task<List<GenreDto>> GetGenresAsync();

        Task<List<CertificateDto>> GetCertificatesAsync();
        Task<int> GetTheaterIdByUserIdAsync(int? userId = null);
        Task<List<MovieCardDto>> GetMoviesAsync(int? userId = null);

        Task<CreateMovieRequest?> GetMovieByIdAsync(int movieId, int? userId = null);

        Task UpdateMovieAsync(IDbConnection connection, IDbTransaction transaction, MoviesDb movies);

        Task DeleteMovieGenresAsync(IDbConnection connection, IDbTransaction transaction, int movieId);
    }
}
