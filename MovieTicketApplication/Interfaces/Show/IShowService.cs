using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Show;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieTicketBookingApplication.Interfaces.Show
{
    public interface IShowService
    {
        Task<IEnumerable<ShowListDto>> GetShowsListAsync(int screenId, DateTime date);
        Task<CreateShowDto?> GetShowForEditAsync(int showId);
        Task<bool> CreateShowAsync(CreateShowDto request);
        Task<bool> UpdateShowAsync(CreateShowDto request);
        Task<bool> DeleteShowAsync(int showId);
        Task<List<ShowSeatPriceDto>> GetDefaultSeatPricesForScreenAsync(int screenId);
    }
}
