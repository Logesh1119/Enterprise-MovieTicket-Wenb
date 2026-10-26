using MovieTicketBooking.Domain.Entities;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Theater;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.TheaterCreation;
using MovieTicketBookingDomain.Entities.TheaterAdmin;
using System.Data;

namespace MovieTicketBooking.Application.Interfaces;

public interface ITheaterRepository
{
    Task<int> CreateTheaterAsync(IDbConnection connection,
        IDbTransaction transaction, Theater theater);

    Task CreateTheaterInformationAsync(IDbConnection connection,
        IDbTransaction transaction, TheaterInformation information);

    Task AddTheaterImageAsync(IDbConnection connection,
        IDbTransaction transaction, TheaterImage image);

    Task<List<TheaterCardDto>> GetTheatersAsync(int? ownerUserId = null);

    Task<CreateTheater?> GetTheaterByIdAsync(int theaterId, int? ownerUserId = null);

    Task<bool> UpdateTheaterAsync(CreateTheater theater, int? ownerUserId = null);
}