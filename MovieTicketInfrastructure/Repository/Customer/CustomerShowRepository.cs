using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MovieTicketBooking.Application.DTOs.Customer.Show;
using MovieTicketBooking.Application.Interfaces.Customer;

namespace MovieTicketBooking.Infrastructure.Repository.Customer
{
    public class CustomerShowRepository : ICustomerShowRepository
    {
        private readonly string _connectionString;

        public CustomerShowRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<CustomerSeatLayoutDto> GetShowSeatLayoutAsync(int showId)
        {
            using var connection = CreateConnection();
            
            // 1. Fetch Show Details
            string showQuery = @"
                SELECT 
                    s.Id as ShowId,
                    m.MovieName,
                    t.TheaterName,
                    sc.ScreenName,
                    s.ShowDate,
                    s.ShowTime,
                    sc.Id as ScreenId
                FROM Shows s
                INNER JOIN MoviesDb m ON s.MovieId = m.Id
                INNER JOIN Theaters t ON m.TheaterId = t.Id
                INNER JOIN Screens sc ON s.ScreenId = sc.Id
                WHERE s.Id = @ShowId";
                
            var showInfo = await connection.QueryFirstOrDefaultAsync<dynamic>(showQuery, new { ShowId = showId });
            
            if (showInfo == null) return null;

            var layout = new CustomerSeatLayoutDto
            {
                ShowId = showInfo.ShowId,
                MovieName = showInfo.MovieName,
                TheaterName = showInfo.TheaterName,
                ScreenName = showInfo.ScreenName,
                ShowDate = showInfo.ShowDate.ToString("yyyy-MM-dd"),
                ShowTime = showInfo.ShowTime.ToString()
            };

            // 2. Fetch Prices
            string priceQuery = "SELECT SeatCategoryId, Price FROM ShowSeatPrices WHERE ShowId = @ShowId";
            var prices = (await connection.QueryAsync<dynamic>(priceQuery, new { ShowId = showId }))
                         .ToDictionary(p => (int)p.SeatCategoryId, p => (decimal)p.Price);

            // 3. Fetch Booked Seats
            string bookedQuery = @"
                SELECT bs.ScreenSeatId 
                FROM BookingSeats bs 
                INNER JOIN Bookings b ON bs.BookingId = b.Id
                WHERE b.ShowId = @ShowId AND b.Status IN ('Confirmed', 'Pending')";
            var bookedSeats = (await connection.QueryAsync<int>(bookedQuery, new { ShowId = showId })).ToHashSet();

            // 4. Fetch Screen Rows and Seats
            string seatQuery = @"
                SELECT 
                    sr.Id as RowId,
                    sr.RowName,
                    sr.RowType,
                    sr.DisplayOrder,
                    sr.SeatCategoryId,
                    cat.CategoryName,
                    ss.Id as SeatId,
                    ss.SeatNo,
                    ss.SeatLabel,
                    ss.SeatType
                FROM ScreenRows sr
                LEFT JOIN SeatCategories cat ON sr.SeatCategoryId = cat.Id
                LEFT JOIN ScreenSeats ss ON sr.Id = ss.ScreenRowId
                WHERE sr.ScreenId = @ScreenId AND sr.Status = 1
                ORDER BY sr.DisplayOrder, ss.SeatNo";

            var rowDict = new Dictionary<int, CustomerRowDto>();

            await connection.QueryAsync<dynamic, dynamic, CustomerRowDto>(
                seatQuery,
                (row, seat) =>
                {
                    if (!rowDict.TryGetValue((int)row.RowId, out var currentRow))
                    {
                        currentRow = new CustomerRowDto
                        {
                            RowId = (int)row.RowId,
                            RowName = row.RowName,
                            RowType = row.RowType,
                            DisplayOrder = (int)row.DisplayOrder
                        };
                        rowDict.Add(currentRow.RowId, currentRow);
                    }

                    if (seat != null && seat.SeatId != null)
                    {
                        int seatId = (int)seat.SeatId;
                        int? categoryId = row.SeatCategoryId;
                        decimal price = 0;
                        if (categoryId.HasValue && prices.TryGetValue(categoryId.Value, out var p))
                        {
                            price = p;
                        }

                        currentRow.Seats.Add(new CustomerSeatDto
                        {
                            SeatId = seatId,
                            SeatNo = (int)seat.SeatNo,
                            SeatLabel = seat.SeatLabel,
                            SeatType = seat.SeatType,
                            SeatCategoryId = categoryId ?? 0,
                            CategoryName = row.CategoryName ?? "",
                            Price = price,
                            IsBooked = bookedSeats.Contains(seatId)
                        });
                    }
                    return currentRow;
                },
                new { ScreenId = showInfo.ScreenId },
                splitOn: "SeatId"
            );

            layout.Rows = rowDict.Values.OrderBy(r => r.DisplayOrder).ToList();
            return layout;
        }
    }
}
