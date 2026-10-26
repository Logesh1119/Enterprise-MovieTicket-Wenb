using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MovieTicketBooking.Application.DTOs.Customer.Booking;
using MovieTicketBooking.Application.Interfaces.Booking;

namespace MovieTicketBooking.Infrastructure.Repository.Booking
{
    public class BookingRepository : IBookingRepository
    {
        private readonly string _connectionString;

        public BookingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<int> CreateBookingAsync(int userId, CreateBookingDto request)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 1. Verify show exists
                string showQuery = "SELECT Id FROM Shows WHERE Id = @ShowId";
                var showId = await connection.QueryFirstOrDefaultAsync<int?>(showQuery, new { request.ShowId }, transaction);
                if (!showId.HasValue) throw new Exception("Show not found.");

                // 2. Fetch prices for the show
                string priceQuery = "SELECT SeatCategoryId, Price FROM ShowSeatPrices WHERE ShowId = @ShowId";
                var prices = (await connection.QueryAsync<dynamic>(priceQuery, new { request.ShowId }, transaction))
                             .ToDictionary(p => (int)p.SeatCategoryId, p => (decimal)p.Price);

                // 3. Check if seats are already booked
                string checkBookedQuery = @"
                    SELECT COUNT(*) 
                    FROM BookingSeats bs
                    INNER JOIN Bookings b ON bs.BookingId = b.Id
                    WHERE b.ShowId = @ShowId 
                      AND bs.ScreenSeatId IN @SeatIds 
                      AND b.Status IN ('Confirmed', 'Pending')";
                var bookedCount = await connection.ExecuteScalarAsync<int>(checkBookedQuery, new { request.ShowId, SeatIds = request.ScreenSeatIds }, transaction);
                
                if (bookedCount > 0) throw new Exception("One or more selected seats are already booked.");

                // 4. Calculate total and prepare seat inserts
                decimal totalAmount = 0;
                var seatInserts = new List<dynamic>();

                string getSeatsQuery = @"
                    SELECT ss.Id, sr.SeatCategoryId 
                    FROM ScreenSeats ss
                    INNER JOIN ScreenRows sr ON ss.ScreenRowId = sr.Id
                    WHERE ss.Id IN @SeatIds";
                var seats = await connection.QueryAsync<dynamic>(getSeatsQuery, new { SeatIds = request.ScreenSeatIds }, transaction);

                foreach (var seat in seats)
                {
                    int seatId = (int)seat.Id;
                    int? catId = seat.SeatCategoryId;
                    decimal price = 0;
                    if (catId.HasValue && prices.TryGetValue(catId.Value, out var p))
                    {
                        price = p;
                    }
                    totalAmount += price;
                    seatInserts.Add(new { ScreenSeatId = seatId, Price = price });
                }

                // 5. Insert Booking
                string insertBookingQuery = @"
                    INSERT INTO Bookings (UserId, ShowId, BookingDate, BookingType, TotalAmount, Status)
                    OUTPUT INSERTED.Id
                    VALUES (@UserId, @ShowId, @BookingDate, 'Online', @TotalAmount, 'Pending')";
                
                int bookingId = await connection.ExecuteScalarAsync<int>(insertBookingQuery, new 
                {
                    UserId = userId,
                    ShowId = request.ShowId,
                    BookingDate = DateTime.UtcNow,
                    TotalAmount = totalAmount
                }, transaction);

                // 6. Insert Booking Seats
                string insertSeatsQuery = @"
                    INSERT INTO BookingSeats (BookingId, ScreenSeatId, Price)
                    VALUES (@BookingId, @ScreenSeatId, @Price)";
                
                foreach (var s in seatInserts)
                {
                    await connection.ExecuteAsync(insertSeatsQuery, new { BookingId = bookingId, ScreenSeatId = s.ScreenSeatId, Price = s.Price }, transaction);
                }

                transaction.Commit();
                return bookingId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<BookingSummaryDto> GetBookingSummaryAsync(int bookingId, int userId)
        {
            using var connection = CreateConnection();
            string query = @"
                SELECT 
                    b.Id as BookingId,
                    m.MovieName,
                    t.TheaterName,
                    sc.ScreenName,
                    s.ShowDate,
                    s.ShowTime,
                    b.TotalAmount,
                    b.Status
                FROM Bookings b
                INNER JOIN Shows s ON b.ShowId = s.Id
                INNER JOIN MoviesDb m ON s.MovieId = m.Id
                INNER JOIN Theaters t ON m.TheaterId = t.Id
                INNER JOIN Screens sc ON s.ScreenId = sc.Id
                WHERE b.Id = @BookingId AND b.UserId = @UserId";
                
            var summary = await connection.QueryFirstOrDefaultAsync<BookingSummaryDto>(query, new { BookingId = bookingId, UserId = userId });
            
            if (summary != null)
            {
                string seatsQuery = @"
                    SELECT ss.SeatLabel 
                    FROM BookingSeats bs
                    INNER JOIN ScreenSeats ss ON bs.ScreenSeatId = ss.Id
                    WHERE bs.BookingId = @BookingId";
                var labels = await connection.QueryAsync<string>(seatsQuery, new { BookingId = bookingId });
                summary.SeatLabels = labels.ToList();
            }
            return summary;
        }

        public async Task<IEnumerable<BookingSummaryDto>> GetUserBookingsAsync(int userId)
        {
            using var connection = CreateConnection();
            string query = @"
                SELECT 
                    b.Id as BookingId,
                    m.MovieName,
                    t.TheaterName,
                    sc.ScreenName,
                    s.ShowDate,
                    s.ShowTime,
                    b.TotalAmount,
                    b.Status
                FROM Bookings b
                INNER JOIN Shows s ON b.ShowId = s.Id
                INNER JOIN MoviesDb m ON s.MovieId = m.Id
                INNER JOIN Theaters t ON m.TheaterId = t.Id
                INNER JOIN Screens sc ON s.ScreenId = sc.Id
                WHERE b.UserId = @UserId
                ORDER BY b.BookingDate DESC";

            var bookings = (await connection.QueryAsync<BookingSummaryDto>(query, new { UserId = userId })).ToList();
            
            if (bookings.Any())
            {
                var bookingIds = bookings.Select(b => b.BookingId).ToList();
                string seatsQuery = @"
                    SELECT bs.BookingId, ss.SeatLabel 
                    FROM BookingSeats bs
                    INNER JOIN ScreenSeats ss ON bs.ScreenSeatId = ss.Id
                    WHERE bs.BookingId IN @BookingIds";
                var seatLabels = await connection.QueryAsync<dynamic>(seatsQuery, new { BookingIds = bookingIds });
                
                var seatDict = seatLabels.GroupBy(x => (int)x.BookingId)
                                         .ToDictionary(g => g.Key, g => g.Select(x => (string)x.SeatLabel).ToList());
                
                foreach (var b in bookings)
                {
                    if (seatDict.TryGetValue(b.BookingId, out var labels))
                    {
                        b.SeatLabels = labels;
                    }
                }
            }

            return bookings;
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, int userId, string status)
        {
            using var connection = CreateConnection();
            string query = "UPDATE Bookings SET Status = @Status WHERE Id = @BookingId AND UserId = @UserId";
            int rows = await connection.ExecuteAsync(query, new { Status = status, BookingId = bookingId, UserId = userId });
            return rows > 0;
        }
    }
}
