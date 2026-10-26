using MovieTicketBookingDomain.Entities.Shows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MovieTicketBookingApplication.Interfaces.Show
{
    public interface IShowRepository
    {
        Task<int> CreateShowAsync(IDbConnection connection, IDbTransaction transaction, Shows show);
        Task UpdateShowAsync(IDbConnection connection, IDbTransaction transaction, Shows show);
        Task DeleteShowAsync(IDbConnection connection, IDbTransaction transaction, int showId);
        Task CreateShowSeatPriceAsync(IDbConnection connection, IDbTransaction transaction, ShowSeatPrices price);
        Task DeleteShowSeatPricesAsync(IDbConnection connection, IDbTransaction transaction, int showId);
        Task<IEnumerable<Shows>> GetShowsByScreenAndDateAsync(int screenId, DateTime date);
        Task<Shows?> GetShowByIdAsync(int id);
        Task<IEnumerable<ShowSeatPrices>> GetShowSeatPricesAsync(int showId);
    }
}
