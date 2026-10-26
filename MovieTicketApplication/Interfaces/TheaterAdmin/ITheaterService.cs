using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Theater;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.TheaterCreation;

namespace MovieTicketBooking.Application.Interfaces;

public interface ITheaterService
{
    Task<bool> CreateTheaterAsync(CreateTheater request);

    Task<List<TheaterCardDto>> GetTheatersAsync();

    Task<CreateTheater?> GetTheaterByIdAsync(int theaterId);

    Task<bool> UpdateTheaterAsync(CreateTheater theater);
}