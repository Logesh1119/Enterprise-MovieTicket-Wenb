using Microsoft.AspNetCore.Http;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBooking.Domain.Entities;
using MovieTicketBooking.Infrastructure.Repositories;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Movies;
using MovieTicketBookingApplication.Interfaces.Movie;
using MovieTicketBookingApplication.Interfaces.Movies;
using MovieTicketBookingDomain.Entities.Movies;
using MovieTicketBookingInfrastructureData;
using System.Security.Claims;


namespace MovieTicketBookingInfrastructure.Service.Movies
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _repository;
        private readonly IMediaRepository _mediaRepository;
        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MovieService(IMovieRepository repository, IMediaRepository mediaRepository, SqlConnectionFactory connectionFactory, IHttpContextAccessor httpContext)
        {
            _repository = repository;
            _mediaRepository = mediaRepository;
            _connectionFactory = connectionFactory;
            _httpContextAccessor = httpContext;
        }

        public async Task<List<GenreDto>> GetGenresAsync()
        {
            return await _repository.GetGenresAsync();
        }

        public async Task<List<CertificateDto>> GetCertificatesAsync()
        {
            return await _repository.GetCertificatesAsync();
        }

        public async Task<bool> CreateMovieAsync(CreateMovieRequest request, int userId)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity!.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            string? userIdClaim =
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int theaterAdminId))
            {
                throw new UnauthorizedAccessException("Invalid User.");
            }

            using var connection = _connectionFactory.CreateConnection();


            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Step 1
                int theaterId = request.TheaterId > 0 ? request.TheaterId : await _repository.GetTheaterIdByUserIdAsync(userId);

                var movie = new MoviesDb
                {
                    TheaterId = theaterId,

                    MovieName = request.MovieName,

                    MovieCode = request.MovieCode,

                    MovieDescription = request.MovieDescription,

                    TrailerUrl = request.TrailerUrl,

                    TeaserUrl = request.TeaserUrl,

                    Duration = request.Duration,

                    ReleaseDate = request.ReleaseDate,

                    CertificateId = request.CertificateId,

                    Status = request.Status,

                    Tags = request.Tags
                };

                // Step 4
                int movieId = await _repository.CreateMovieAsync(connection, transaction, movie);

                if (request.Images != null && request.Images.Any())
                {
                    int displayOrder = 1;

                    foreach (var image in request.Images)
                    {
                        Media media =
                            await _mediaRepository.SaveImageAsync(image, "Movies");

                        int mediaId =
                            await _mediaRepository.CreateAsync(
                                connection,
                                transaction,
                                media);

                        await _repository.AddMovieImageAsync(
                                connection,
                                transaction,
                                new MovieImages
                                {
                                    MovieId = movieId,
                                    MediaId = mediaId,
                                    DisplayOrder = displayOrder,
                                    IsPrimary = displayOrder == 1,
                                });
                        displayOrder++;
                    }
                }

                foreach (int genreId in request.GenreId)
                {
                    var movieGenre = new MovieGenres
                    {
                        MovieId = movieId,
                        GenreId = genreId
                    };

                    await _repository.CreateMovieGenreAsync(
                        connection,
                        transaction,
                        movieGenre);
                }

                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private int? GetUserIdForQuery()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity!.IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated.");

            if (user.IsInRole("SuperAdmin"))
            {
                return null;
            }

            string? userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException("Invalid User.");

            return userId;
        }

        public async Task<List<MovieCardDto>> GetMoviesAsync()
        {
            int? userId = GetUserIdForQuery();
            return await _repository.GetMoviesAsync(userId);
        }

        public async Task<CreateMovieRequest?> GetMovieByIdAsync(int movieId)
        {
            int? userId = GetUserIdForQuery();
            return await _repository.GetMovieByIdAsync(movieId, userId);
        }

        public async Task<bool> UpdateMovieAsync(int movieId, CreateMovieRequest request)
        {
            int? userId = GetUserIdForQuery();

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var existing = await _repository.GetMovieByIdAsync(movieId, userId);
                if (existing == null) throw new UnauthorizedAccessException("Movie not found or access denied.");

                var movie = new MoviesDb
                {
                    Id = movieId,
                    MovieName = request.MovieName,
                    MovieCode = request.MovieCode,
                    MovieDescription = request.MovieDescription,
                    TrailerUrl = request.TrailerUrl,
                    TeaserUrl = request.TeaserUrl,
                    Duration = request.Duration,
                    ReleaseDate = request.ReleaseDate,
                    CertificateId = request.CertificateId,
                    Status = request.Status,
                    Tags = request.Tags
                };

                await _repository.UpdateMovieAsync(connection, transaction, movie);

                // Update genres
                await _repository.DeleteMovieGenresAsync(connection, transaction, movieId);
                
                if (request.GenreId != null)
                {
                    foreach (int genreId in request.GenreId)
                    {
                        var movieGenre = new MovieGenres
                        {
                            MovieId = movieId,
                            GenreId = genreId
                        };
                        await _repository.CreateMovieGenreAsync(connection, transaction, movieGenre);
                    }
                }

                if (request.Images != null && request.Images.Any())
                {
                    // Optionally delete old images or just append. We'll just append for now as per Create flow.
                    int displayOrder = 1;
                    foreach (var image in request.Images)
                    {
                        Media media = await _mediaRepository.SaveImageAsync(image, "Movies");
                        int mediaId = await _mediaRepository.CreateAsync(connection, transaction, media);
                        await _repository.AddMovieImageAsync(
                                connection,
                                transaction,
                                new MovieImages
                                {
                                    MovieId = movieId,
                                    MediaId = mediaId,
                                    DisplayOrder = displayOrder,
                                    IsPrimary = displayOrder == 1,
                                });
                        displayOrder++;
                    }
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
