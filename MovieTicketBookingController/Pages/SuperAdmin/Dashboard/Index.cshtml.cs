using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.DTOs.SuperAdmin.Dashboard;
using MovieTicketBooking.Application.Interfaces.SuperAdmin.Dashboard;
using System.Threading.Tasks;

namespace MovieTicketBooking.Pages.SuperAdmin.Dashboard
{
    [Authorize(Roles = "SuperAdmin")]
    public class DashboardModel : PageModel
    {
        private readonly ISuperAdminDashboardService _dashboardService;

        public DashboardModel(ISuperAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public SuperAdminDashboardDataDto DashboardData { get; set; } = new SuperAdminDashboardDataDto();

        public async Task<IActionResult> OnGetAsync()
        {
            DashboardData = await _dashboardService.GetDashboardDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetRefreshAjaxAsync()
        {
            var data = await _dashboardService.GetDashboardDataAsync();
            return new JsonResult(data);
        }
    }
}
