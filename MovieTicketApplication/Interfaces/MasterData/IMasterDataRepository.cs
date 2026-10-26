using MovieTicketBookingDomain.Entities.MasterData;
using MovieTicketBookingDomain.Entities.Screen;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieTicketBooking.Application.Interfaces
{
    public interface IMasterDataRepository
    {
        Task<List<Genres>> GetGenresAsync();
        Task<bool> CreateGenreAsync(Genres genre);
        Task<bool> UpdateGenreAsync(Genres genre);
        Task<bool> DeleteGenreAsync(int id);
        Task<bool> IsGenreReferencedAsync(int id);

        Task<List<SeatCategories>> GetSeatCategoriesAsync();
        Task<bool> CreateSeatCategoryAsync(SeatCategories category);
        Task<bool> UpdateSeatCategoryAsync(SeatCategories category);
        Task<bool> DeleteSeatCategoryAsync(int id);
        Task<bool> IsSeatCategoryReferencedAsync(int id);

        Task<List<AmenityMaster>> GetAmenitiesAsync();
        Task<bool> CreateAmenityAsync(AmenityMaster amenity);
        Task<bool> UpdateAmenityAsync(AmenityMaster amenity);
        Task<bool> DeleteAmenityAsync(int id);
        Task<bool> IsAmenityReferencedAsync(int id);

        Task<List<Certificates>> GetCertificatesAsync();
        Task<bool> CreateCertificateAsync(Certificates certificate);
        Task<bool> UpdateCertificateAsync(Certificates certificate);
        Task<bool> DeleteCertificateAsync(int id);
        Task<bool> IsCertificateReferencedAsync(int id);
    }
}
