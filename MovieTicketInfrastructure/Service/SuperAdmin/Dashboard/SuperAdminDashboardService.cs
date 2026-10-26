using MovieTicketBooking.Application.DTOs.SuperAdmin.Dashboard;
using MovieTicketBooking.Application.Interfaces.SuperAdmin.Dashboard;
using System.Threading.Tasks;

namespace MovieTicketBooking.Infrastructure.Service.SuperAdmin.Dashboard
{
    public class SuperAdminDashboardService : ISuperAdminDashboardService
    {
        private readonly ISuperAdminDashboardRepository _repository;

        public SuperAdminDashboardService(ISuperAdminDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<SuperAdminDashboardDataDto> GetDashboardDataAsync()
        {
            return await _repository.GetDashboardDataAsync();
        }
    }
}
