using Dapper;
using MovieTicketBooking.Application.DTOs.TheaterAdmin.Dashboard;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBookingInfrastructureData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketBooking.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public DashboardRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<DashboardDataDto> GetDashboardDataAsync(int theaterAdminId)
        {
            var data = new DashboardDataDto();
            using var connection = _connectionFactory.CreateConnection();

            // 1. Get Metrics
            string metricsSql = @"
                SELECT 
                    (SELECT COUNT(DISTINCT b.Id) 
                     FROM Bookings b 
                     JOIN Shows s ON b.ShowId = s.Id 
                     JOIN Screens sc ON s.ScreenId = sc.Id 
                     JOIN Theaters t ON sc.TheaterId = t.Id 
                     WHERE t.OwnerUserId = @AdminId AND CAST(b.BookingDate AS DATE) = CAST(GETUTCDATE() AS DATE) AND b.Status = 'Confirmed') AS TodayBookings,
                     
                    (SELECT COUNT(sc.Id) 
                     FROM Screens sc 
                     JOIN Theaters t ON sc.TheaterId = t.Id 
                     WHERE t.OwnerUserId = @AdminId AND sc.Status = 1) AS ActiveScreens,
                     
                    (SELECT COUNT(sc.Id) 
                     FROM Screens sc 
                     JOIN Theaters t ON sc.TheaterId = t.Id 
                     WHERE t.OwnerUserId = @AdminId) AS TotalScreens,
                     
                    (SELECT COUNT(t.Id) 
                     FROM Theaters t 
                     WHERE t.OwnerUserId = @AdminId) AS TotalTheaters,
                     
                    (SELECT COUNT(DISTINCT m.Id) 
                     FROM MoviesDb m 
                     JOIN Shows s ON m.Id = s.MovieId 
                     JOIN Screens sc ON s.ScreenId = sc.Id 
                     JOIN Theaters t ON sc.TheaterId = t.Id 
                     WHERE t.OwnerUserId = @AdminId AND m.Status = 1) AS ActiveMovies,
                     
                    (SELECT ISNULL(SUM(b.TotalAmount), 0) 
                     FROM Bookings b 
                     JOIN Shows s ON b.ShowId = s.Id 
                     JOIN Screens sc ON s.ScreenId = sc.Id 
                     JOIN Theaters t ON sc.TheaterId = t.Id 
                     WHERE t.OwnerUserId = @AdminId AND b.BookingDate >= DATEADD(day, -7, GETUTCDATE()) AND b.Status = 'Confirmed') AS WeeklyRevenue
            ";

            data.Metrics = await connection.QueryFirstOrDefaultAsync<DashboardMetricsDto>(metricsSql, new { AdminId = theaterAdminId }) ?? new DashboardMetricsDto();

            // 2. Get Chart Data (Current Year)
            string chartSql = @"
                SELECT 
                    MONTH(b.BookingDate) AS [Month],
                    b.BookingType,
                    SUM(b.TotalAmount) AS TotalRevenue
                FROM Bookings b
                JOIN Shows s ON b.ShowId = s.Id
                JOIN Screens sc ON s.ScreenId = sc.Id
                JOIN Theaters t ON sc.TheaterId = t.Id
                WHERE t.OwnerUserId = @AdminId AND YEAR(b.BookingDate) = YEAR(GETUTCDATE()) AND b.Status = 'Confirmed'
                GROUP BY MONTH(b.BookingDate), b.BookingType
            ";

            var chartRecords = await connection.QueryAsync(chartSql, new { AdminId = theaterAdminId });
            
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

            // 3. Get Recent Activities
            string activitySql = @"
                SELECT TOP 10 ActionType, ActionDescription, CreatedAt
                FROM ActivityLogs
                WHERE TheaterAdminId = @AdminId
                ORDER BY CreatedAt DESC
            ";
            
            var activities = await connection.QueryAsync(activitySql, new { AdminId = theaterAdminId });
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

            // 4. Get Top 5 Movies
            // Calculate Revenue and Occupancy per movie.
            // Occupancy = Total Booked Seats / (Total Shows * Seats per Screen)
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
                    JOIN Theaters t ON sc.TheaterId = t.Id
                    LEFT JOIN Bookings b ON b.ShowId = s.Id AND b.Status = 'Confirmed'
                    LEFT JOIN BookingSeats bs ON bs.BookingId = b.Id
                    WHERE t.OwnerUserId = @AdminId
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

            var topMovies = await connection.QueryAsync<TopMovieDto>(topMoviesSql, new { AdminId = theaterAdminId });
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
