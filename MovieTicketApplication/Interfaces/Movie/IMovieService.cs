using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Movies;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTicketBookingApplication.Interfaces.Movies
{
    public interface IMovieService
    {
        Task<bool> CreateMovieAsync(CreateMovieRequest request, int userId);

        Task<List<GenreDto>> GetGenresAsync();

        Task<List<CertificateDto>> GetCertificatesAsync();

        Task<List<MovieCardDto>> GetMoviesAsync();
        
        Task<CreateMovieRequest?> GetMovieByIdAsync(int movieId);

        Task<bool> UpdateMovieAsync(int movieId, CreateMovieRequest request);
    }
}
