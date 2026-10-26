using Microsoft.AspNetCore.Http;
using MovieTicketBooking.Domain.Entities;
using System.Data;

namespace MovieTicketBooking.Application.Interfaces
{
    public interface IMediaRepository
    {
        Task<Media> SaveImageAsync(IFormFile file, string folderName);
        Task<int> CreateAsync(IDbConnection connection, IDbTransaction transaction, Media media);
        Task<int> CreateImageAsync(Media media);
        Task<Media?> GetByIdAsync(int id);

        Task DeleteAsync(int id);
    }
}