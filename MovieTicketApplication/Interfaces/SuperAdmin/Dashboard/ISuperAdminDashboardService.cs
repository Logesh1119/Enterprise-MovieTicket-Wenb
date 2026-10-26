using MovieTicketBooking.Application.DTOs.SuperAdmin.Dashboard;
using System.Threading.Tasks;

namespace MovieTicketBooking.Application.Interfaces.SuperAdmin.Dashboard
{
    public interface ISuperAdminDashboardService
    {
        Task<SuperAdminDashboardDataDto> GetDashboardDataAsync();
    }
}
