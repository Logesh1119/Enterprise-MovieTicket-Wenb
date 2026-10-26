using Dapper;
using MovieTicketBooking.Application.DTOs.SuperAdmin.Dashboard;
using MovieTicketBooking.Application.DTOs.TheaterAdmin.Dashboard;
using MovieTicketBooking.Application.Interfaces.SuperAdmin.Dashboard;
using MovieTicketBookingInfrastructureData;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketBooking.Infrastructure.Repository.SuperAdmin.Dashboard
{
    public class SuperAdminDashboardRepository : ISuperAdminDashboardRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public SuperAdminDashboardRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<SuperAdminDashboardDataDto> GetDashboardDataAsync()
        {
            var data = new SuperAdminDashboardDataDto();
            using var connection = _connectionFactory.CreateConnection();

            // 1. Get Global Metrics
            string metricsSql = @"
                SELECT 
                    (SELECT COUNT(Id) FROM Users) AS TotalUsers,
                    (SELECT COUNT(Id) FROM Theaters) AS TotalTheaters,
                    (SELECT COUNT(Id) FROM MoviesDb) AS TotalMovies,
                    (SELECT COUNT(Id) FROM Shows) AS TotalShows,
                    (SELECT COUNT(Id) FROM Bookings WHERE CAST(BookingDate AS DATE) = CAST(GETUTCDATE() AS DATE) AND Status = 'Confirmed') AS TodayBookings,
                    (SELECT ISNULL(SUM(TotalAmount), 0) FROM Bookings WHERE BookingDate >= DATEADD(day, -7, GETUTCDATE()) AND Status = 'Confirmed') AS WeeklyRevenue
            ";

            data.Metrics = await connection.QueryFirstOrDefaultAsync<SuperAdminDashboardMetricsDto>(metricsSql) ?? new SuperAdminDashboardMetricsDto();

            // 2. Get Chart Data (Current Year - Global)
            string chartSql = @"
                SELECT 
                    MONTH(BookingDate) AS [Month],
                    BookingType,
                    SUM(TotalAmount) AS TotalRevenue
                FROM Bookings
                WHERE YEAR(BookingDate) = YEAR(GETUTCDATE()) AND Status = 'Confirmed'
                GROUP BY MONTH(BookingDate), BookingType
            ";

            var chartRecords = await connection.QueryAsync(chartSql);
            
            var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            data.ChartData.Labels = months.ToList();
            data.ChartData.OnlineBookings = new decimal[12].ToList();
            data.ChartData.OnSiteBookings = new decimal[12].ToList();

            foreach (var record in chartRecords)
            {
                int monthIndex = (int)record.Month - 1;
                string type = (string)record.BookingType;
                decimal rev = (decimal)record.TotalRevenue;

                if (type == "Online") data.ChartData.OnlineBookings[monthIndex] = rev;
                else if (type == "On-site") data.ChartData.OnSiteBookings[monthIndex] = rev;
            }

            // 3. Get Recent Activities (Global)
            string activitySql = @"
                SELECT TOP 10 ActionType, ActionDescription, CreatedAt
                FROM ActivityLogs
                ORDER BY CreatedAt DESC
            ";
            
            var activities = await connection.QueryAsync(activitySql);
            foreach (var act in activities)
            {
                var date = (DateTime)act.CreatedAt;
                string timeString = "";
                var diff = DateTime.UtcNow - date;
                if (diff.TotalMinutes < 60) timeString = $"{(int)diff.TotalMinutes} mins ago";
                else if (diff.TotalHours < 24) timeString = $"{(int)diff.TotalHours} hours ago";
                else timeString = $"{(int)diff.TotalDays} days ago";

                data.RecentActivities.Add(new ActivityLogDto
                {
                    ActionType = act.ActionType,
                    ActionDescription = act.ActionDescription,
                    CreatedAtString = timeString
                });
            }

            // 4. Get Top 5 Movies Globally
            string topMoviesSql = @"
                WITH MovieStats AS (
                    SELECT 
                        m.Id AS MovieId,
                        m.MovieName,
                        m.Duration,
                        YEAR(m.ReleaseDate) AS ReleaseYear,
                        s.Language,
                        s.Format,
                        COUNT(DISTINCT sc.Id) AS ScreenCount,
                        ISNULL(SUM(bs.Price), 0) AS Revenue,
                        COUNT(DISTINCT bs.Id) AS BookedSeats,
                        (SELECT COUNT(sr.Id) 
                         FROM ScreenSeats sr 
                         JOIN ScreenRows row ON sr.ScreenRowId = row.Id 
                         WHERE row.ScreenId = sc.Id) * COUNT(DISTINCT s.Id) AS TotalCapacity
                    FROM MoviesDb m
                    JOIN Shows s ON m.Id = s.MovieId
                    JOIN Screens sc ON s.ScreenId = sc.Id
                    LEFT JOIN Bookings b ON b.ShowId = s.Id AND b.Status = 'Confirmed'
                    LEFT JOIN BookingSeats bs ON bs.BookingId = b.Id
                    GROUP BY m.Id, m.MovieName, m.Duration, m.ReleaseDate, s.Language, s.Format, sc.Id
                )
                SELECT TOP 5
                    MovieName,
                    Duration,
                    CAST(ReleaseYear AS NVARCHAR) AS ReleaseYear,
                    Language,
                    Format,
                    ScreenCount,
                    Revenue,
                    CASE WHEN TotalCapacity > 0 THEN (CAST(BookedSeats AS DECIMAL) / TotalCapacity) * 100 ELSE 0 END AS Occupancy,
                    (SELECT TOP 1 FilePath FROM Media med JOIN MovieImages mi ON med.Id = mi.MediaId WHERE mi.MovieId = MovieStats.MovieId AND mi.IsPrimary = 1) AS PosterUrl
                FROM MovieStats
                ORDER BY Revenue DESC
            ";

            var topMovies = await connection.QueryAsync<TopMovieDto>(topMoviesSql);
            int rank = 1;
            foreach (var m in topMovies)
            {
                m.Rank = rank++;
                data.TopMovies.Add(m);
            }

            return data;
        }
    }
}
