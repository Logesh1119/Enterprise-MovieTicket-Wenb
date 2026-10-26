using MovieTicketBooking.Application.DTOs.TheaterAdmin.Dashboard;
using System.Collections.Generic;

namespace MovieTicketBooking.Application.DTOs.SuperAdmin.Dashboard
{
    public class SuperAdminDashboardMetricsDto
    {
        public int TotalUsers { get; set; }
        public int TotalTheaters { get; set; }
        public int TotalMovies { get; set; }
        public int TotalShows { get; set; }
        public int TodayBookings { get; set; }
        public decimal WeeklyRevenue { get; set; }
    }

    public class SuperAdminDashboardDataDto
    {
        public SuperAdminDashboardMetricsDto Metrics { get; set; } = new SuperAdminDashboardMetricsDto();
        public ChartDataDto ChartData { get; set; } = new ChartDataDto();
        public List<ActivityLogDto> RecentActivities { get; set; } = new List<ActivityLogDto>();
        public List<TopMovieDto> TopMovies { get; set; } = new List<TopMovieDto>();
    }
}
