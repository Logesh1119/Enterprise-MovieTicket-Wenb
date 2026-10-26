using MovieTicketBooking.Application.Interfaces;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Movies;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.MasterData;
using MovieTicketBookingDomain.Entities.MasterData;
using MovieTicketBookingDomain.Entities.Screen;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketBooking.Infrastructure.Services
{
    public class MasterDataService : IMasterDataService
    {
        private readonly IMasterDataRepository _repository;

        public MasterDataService(IMasterDataRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GenreDto>> GetGenresAsync()
        {
            var res = await _repository.GetGenresAsync();
            return res.Select(x => new GenreDto { Id = x.Id, GenreName = x.GenreName }).ToList();
        }
        public async Task<bool> CreateGenreAsync(GenreDto genre) => await _repository.CreateGenreAsync(new Genres { GenreName = genre.GenreName });
        public async Task<bool> UpdateGenreAsync(GenreDto genre) => await _repository.UpdateGenreAsync(new Genres { Id = genre.Id, GenreName = genre.GenreName });
        public async Task<bool> DeleteGenreAsync(int id)
        {
            if (await _repository.IsGenreReferencedAsync(id)) return false;
            return await _repository.DeleteGenreAsync(id);
        }

        public async Task<List<SeatCategoryDto>> GetSeatCategoriesAsync()
        {
            var res = await _repository.GetSeatCategoriesAsync();
            return res.Select(x => new SeatCategoryDto { Id = x.Id, CategoryName = x.CategoryName, ColorCode = x.ColorCode ?? "" }).ToList();
        }
        public async Task<bool> CreateSeatCategoryAsync(SeatCategoryDto category) => await _repository.CreateSeatCategoryAsync(new SeatCategories { CategoryName = category.CategoryName, ColorCode = category.ColorCode });
        public async Task<bool> UpdateSeatCategoryAsync(SeatCategoryDto category) => await _repository.UpdateSeatCategoryAsync(new SeatCategories { Id = category.Id, CategoryName = category.CategoryName, ColorCode = category.ColorCode });
        public async Task<bool> DeleteSeatCategoryAsync(int id)
        {
            if (await _repository.IsSeatCategoryReferencedAsync(id)) return false;
            return await _repository.DeleteSeatCategoryAsync(id);
        }

        public async Task<List<AmenityDto>> GetAmenitiesAsync()
        {
            var res = await _repository.GetAmenitiesAsync();
            return res.Select(x => new AmenityDto { Id = x.Id, AmenityName = x.AmenityName }).ToList();
        }
        public async Task<bool> CreateAmenityAsync(AmenityDto amenity) => await _repository.CreateAmenityAsync(new AmenityMaster { AmenityName = amenity.AmenityName });
        public async Task<bool> UpdateAmenityAsync(AmenityDto amenity) => await _repository.UpdateAmenityAsync(new AmenityMaster { Id = amenity.Id, AmenityName = amenity.AmenityName });
        public async Task<bool> DeleteAmenityAsync(int id)
        {
            if (await _repository.IsAmenityReferencedAsync(id)) return false;
            return await _repository.DeleteAmenityAsync(id);
        }

        public async Task<List<CertificateDto>> GetCertificatesAsync()
        {
            var res = await _repository.GetCertificatesAsync();
            return res.Select(x => new CertificateDto { Id = x.Id, CertificateName = x.CertificateName }).ToList();
        }
        public async Task<bool> CreateCertificateAsync(CertificateDto certificate) => await _repository.CreateCertificateAsync(new Certificates { CertificateName = certificate.CertificateName });
        public async Task<bool> UpdateCertificateAsync(CertificateDto certificate) => await _repository.UpdateCertificateAsync(new Certificates { Id = certificate.Id, CertificateName = certificate.CertificateName });
        public async Task<bool> DeleteCertificateAsync(int id)
        {
            if (await _repository.IsCertificateReferencedAsync(id)) return false;
            return await _repository.DeleteCertificateAsync(id);
        }
    }
}
