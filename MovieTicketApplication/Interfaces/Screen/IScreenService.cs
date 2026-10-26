using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Movies;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen;
using MovieTicketBookingDomain.Entities.Screen;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieTicketBookingApplication.Interfaces.Screen
{
    public interface IScreenService
    {
        Task<int> CreateScreen(CreateScreen request, int userId);

        Task<List<CreateAmenity>> GetAsync();

        Task<ScreenLayoutDto> SaveScreenLayoutAsync(ScreenLayoutDto request);

        Task<ScreenLayoutDto?> GetScreenLayoutAsync(int screenId);

        Task<List<SeatCategories>> GetSeatCategoriesAsync();

        Task<bool> SaveSeatAllocationAsync(List<SeatAllocationDto> request);

        // ── New Methods for Theater Details & Screen Editing ─────────────
        Task<List<ScreenCardDto>> GetScreensByTheaterIdAsync(int theaterId);

        Task<CreateScreen?> GetScreenByIdAsync(int screenId);

        Task<bool> UpdateScreenAsync(int screenId, CreateScreen request);
    }
}
