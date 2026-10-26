using MovieTicketBooking.Application.DTOs.TheaterAdmin.Dashboard;
using MovieTicketBooking.Application.Interfaces;
using System.Threading.Tasks;

namespace MovieTicketBooking.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repository;

        public DashboardService(IDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<DashboardDataDto> GetDashboardDataAsync(int theaterAdminId)
        {
            return await _repository.GetDashboardDataAsync(theaterAdminId);
        }
    }
}
