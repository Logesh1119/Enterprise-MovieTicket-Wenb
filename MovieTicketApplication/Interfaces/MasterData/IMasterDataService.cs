using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Movies;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.MasterData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieTicketBooking.Application.Interfaces
{
    public interface IMasterDataService
    {
        Task<List<GenreDto>> GetGenresAsync();
        Task<bool> CreateGenreAsync(GenreDto genre);
        Task<bool> UpdateGenreAsync(GenreDto genre);
        Task<bool> DeleteGenreAsync(int id);

        Task<List<SeatCategoryDto>> GetSeatCategoriesAsync();
        Task<bool> CreateSeatCategoryAsync(SeatCategoryDto category);
        Task<bool> UpdateSeatCategoryAsync(SeatCategoryDto category);
        Task<bool> DeleteSeatCategoryAsync(int id);

        Task<List<AmenityDto>> GetAmenitiesAsync();
        Task<bool> CreateAmenityAsync(AmenityDto amenity);
        Task<bool> UpdateAmenityAsync(AmenityDto amenity);
        Task<bool> DeleteAmenityAsync(int id);

        Task<List<CertificateDto>> GetCertificatesAsync();
        Task<bool> CreateCertificateAsync(CertificateDto certificate);
        Task<bool> UpdateCertificateAsync(CertificateDto certificate);
        Task<bool> DeleteCertificateAsync(int id);
    }
}
