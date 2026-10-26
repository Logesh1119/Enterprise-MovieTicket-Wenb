using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MovieTicketBooking.Application.DTOs.Customer.Theater;
using MovieTicketBooking.Application.Interfaces.Customer;

namespace MovieTicketBooking.Infrastructure.Repository.Customer
{
    public class CustomerTheaterRepository : ICustomerTheaterRepository
    {
        private readonly string _connectionString;

        public CustomerTheaterRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<CustomerTheaterDto>> GetAllTheatersAsync()
        {
            using var connection = CreateConnection();
            string query = @"
                SELECT 
                    t.Id as TheaterId,
                    t.TheaterName,
                    ISNULL(ti.City, '') + ', ' + ISNULL(ti.State, '') as Location
                FROM Theaters t
                LEFT JOIN TheaterInformation ti ON t.Id = ti.TheaterId
                WHERE t.Status = 1
                ORDER BY t.TheaterName";
                
            return await connection.QueryAsync<CustomerTheaterDto>(query);
        }

        public async Task<IEnumerable<CustomerTheaterDto>> GetTheatersPlayingMovieAsync(string movieName, DateTime date)
        {
            using var connection = CreateConnection();
            string query = @"
                SELECT 
                    t.Id as TheaterId,
                    t.TheaterName,
                    ISNULL(ti.City, '') + ', ' + ISNULL(ti.State, '') as Location,
                    m.Id as LocalMovieId,
                    s.Id as ShowId,
                    s.ShowTime,
                    s.Language,
                    s.Format,
                    sc.ScreenName,
                    sc.Id as ScreenId
                FROM Theaters t
                LEFT JOIN TheaterInformation ti ON t.Id = ti.TheaterId
                INNER JOIN MoviesDb m ON m.TheaterId = t.Id
                INNER JOIN Shows s ON s.MovieId = m.Id
                INNER JOIN Screens sc ON s.ScreenId = sc.Id
                WHERE m.MovieName = @MovieName 
                  AND s.ShowDate = @Date
                  AND s.Status = 'Published'
                  AND t.Status = 1
                ORDER BY t.TheaterName, s.ShowTime";

            var theaterDict = new Dictionary<int, CustomerTheaterDto>();

            await connection.QueryAsync<CustomerTheaterDto, CustomerShowDto, CustomerTheaterDto>(
                query,
                (theater, show) =>
                {
                    if (!theaterDict.TryGetValue(theater.TheaterId, out var currentTheater))
                    {
                        currentTheater = theater;
                        currentTheater.Shows = new List<CustomerShowDto>();
                        theaterDict.Add(currentTheater.TheaterId, currentTheater);
                    }

                    if (show != null && show.ShowId > 0)
                    {
                        currentTheater.Shows.Add(show);
                    }
                    return currentTheater;
                },
                new { MovieName = movieName, Date = date.Date },
                splitOn: "ShowId"
            );

            return theaterDict.Values.ToList();
        }
    }
}
