using MovieTicketBooking.Application.DTOs.TheaterAdmin.Dashboard;
using System.Threading.Tasks;

namespace MovieTicketBooking.Application.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardDataDto> GetDashboardDataAsync(int theaterAdminId);
    }
}
