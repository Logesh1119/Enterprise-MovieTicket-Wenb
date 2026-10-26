using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Movies;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen;
using MovieTicketBookingDomain.Entities.Movies;
using MovieTicketBookingDomain.Entities.Screen;
using MovieTicketBookingDomain.Entities.Screens;
using System.Data;

namespace MovieTicketBookingApplication.Interfaces.Screen
{
    public interface IScreenRepository
    {
        Task<int> CreateScreenAsync(IDbConnection connection, IDbTransaction transaction, Screens screens);

        Task CreateAmenityAsync(IDbConnection connection, IDbTransaction transaction, Amenities amenities);

        Task<List<CreateAmenity>> GetAmenityAsync();

        Task<int> GetTheaterIdByUserIdAsync(int userId);

        Task<int> CreateScreenRowAsync(IDbConnection connection, IDbTransaction transaction, ScreenRows screenRow);

        Task CreateScreenSeatAsync(IDbConnection connection, IDbTransaction transaction, ScreenSeats screenSeat);

        Task<List<ScreenRows>> GetScreenRowsByScreenIdAsync(int screenId);

        Task<List<ScreenSeats>> GetScreenSeatsByRowIdAsync(int screenRowId);

        Task<List<SeatCategories>> GetSeatCategoriesAsync();

        Task DeleteScreenSeatsAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            int screenId);

        Task DeleteScreenRowsAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            int screenId);

        Task<bool> SaveSeatAllocationAsync(
            List<SeatAllocationDto> request);

        // ── New Methods for Theater Details & Screen Editing ─────────────
        Task<List<ScreenCardDto>> GetScreensByTheaterIdAsync(int theaterId);

        Task<CreateScreen?> GetScreenByIdAsync(int screenId);

        Task<bool> UpdateScreenAsync(int screenId, CreateScreen request);
    }
}
