using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.DTOs.TheaterAdmin.Dashboard;
using MovieTicketBooking.Application.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MovieTicketBooking.Pages.TheaterAdmin.Dashboard
{
    [IgnoreAntiforgeryToken(Order = 1000)]
    public class DashboardModel : PageModel
    {
        private readonly IDashboardService _dashboardService;

        public DashboardModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public DashboardDataDto DashboardData { get; set; } = new DashboardDataDto();

        public async Task<IActionResult> OnGetAsync()
        {
            int theaterAdminId = 5; // Default fallback for development
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int uid))
            {
                theaterAdminId = uid;
            }

            DashboardData = await _dashboardService.GetDashboardDataAsync(theaterAdminId);
            return Page();
        }

        public async Task<IActionResult> OnGetRefreshAjaxAsync()
        {
            int theaterAdminId = 5;
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int uid))
            {
                theaterAdminId = uid;
            }

            var data = await _dashboardService.GetDashboardDataAsync(theaterAdminId);
            return new JsonResult(data);
        }
    }
}
