using System.Collections.Generic;

namespace MovieTicketBooking.Application.DTOs.TheaterAdmin.Dashboard
{
    public class DashboardMetricsDto
    {
        public int TodayBookings { get; set; }
        public int ActiveScreens { get; set; }
        public int TotalScreens { get; set; }
        public int TotalTheaters { get; set; }
        public int ActiveMovies { get; set; }
        public decimal WeeklyRevenue { get; set; }
    }

    public class ChartDataDto
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<decimal> OnlineBookings { get; set; } = new List<decimal>();
        public List<decimal> OnSiteBookings { get; set; } = new List<decimal>();
    }

    public class ActivityLogDto
    {
        public string ActionType { get; set; } = string.Empty;
        public string ActionDescription { get; set; } = string.Empty;
        public string CreatedAtString { get; set; } = string.Empty;
    }

    public class TopMovieDto
    {
        public int Rank { get; set; }
        public string MovieName { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string ReleaseYear { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public int ScreenCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal Occupancy { get; set; }
        public string PosterUrl { get; set; } = string.Empty;
    }

    public class DashboardDataDto
    {
        public DashboardMetricsDto Metrics { get; set; } = new DashboardMetricsDto();
        public ChartDataDto ChartData { get; set; } = new ChartDataDto();
        public List<ActivityLogDto> RecentActivities { get; set; } = new List<ActivityLogDto>();
        public List<TopMovieDto> TopMovies { get; set; } = new List<TopMovieDto>();
    }
}
