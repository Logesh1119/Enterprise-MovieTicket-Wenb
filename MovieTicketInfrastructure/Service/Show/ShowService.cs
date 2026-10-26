using Dapper;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Show;
using MovieTicketBookingApplication.Interfaces.Show;
using MovieTicketBookingDomain.Entities.Shows;
using MovieTicketBookingInfrastructureData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketBookingInfrastructure.Service.Show
{
    public class ShowService : IShowService
    {
        private readonly IShowRepository _showRepository;
        private readonly SqlConnectionFactory _connectionFactory;

        public ShowService(IShowRepository showRepository, SqlConnectionFactory connectionFactory)
        {
            _showRepository = showRepository;
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ShowListDto>> GetShowsListAsync(int screenId, DateTime date)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT 
                    s.Id,
                    s.ScreenId,
                    scr.ScreenName,
                    s.MovieId,
                    m.MovieName,
                    img.FilePath AS MovieImagePath,
                    s.ShowDate,
                    s.ShowTime,
                    s.Language,
                    s.Format,
                    s.Status,
                    (SELECT ISNULL(MIN(Price), 0) FROM ShowSeatPrices WHERE ShowId = s.Id) AS MinPrice,
                    (SELECT ISNULL(MAX(Price), 0) FROM ShowSeatPrices WHERE ShowId = s.Id) AS MaxPrice
                FROM Shows s
                INNER JOIN Screens scr ON s.ScreenId = scr.Id
                INNER JOIN MoviesDb m ON s.MovieId = m.Id
                LEFT JOIN MovieImages mi ON mi.MovieId = m.Id AND mi.IsPrimary = 1
                LEFT JOIN Media img ON img.Id = mi.MediaId
                WHERE s.ScreenId = @ScreenId AND CAST(s.ShowDate AS DATE) = CAST(@ShowDate AS DATE)
                ORDER BY s.ShowTime ASC;";
            
            var results = await connection.QueryAsync<dynamic>(sql, new { ScreenId = screenId, ShowDate = date });
            
            return results.Select(r => new ShowListDto
            {
                Id = r.Id,
                ScreenId = r.ScreenId,
                ScreenName = r.ScreenName,
                MovieId = r.MovieId,
                MovieName = r.MovieName,
                MovieImagePath = r.MovieImagePath ?? "",
                ShowDate = r.ShowDate,
                ShowTime = r.ShowTime,
                Language = r.Language,
                Format = r.Format,
                Status = r.Status,
                PriceRange = r.MinPrice == r.MaxPrice ? $"?{r.MinPrice}" : $"?{r.MinPrice} - ?{r.MaxPrice}",
                Bookings = 0 // Bookings not implemented yet per instructions
            }).ToList();
        }

        public async Task<CreateShowDto?> GetShowForEditAsync(int showId)
        {
            var show = await _showRepository.GetShowByIdAsync(showId);
            if (show == null) return null;

            var prices = await _showRepository.GetShowSeatPricesAsync(showId);
            
            var dto = new CreateShowDto
            {
                Id = show.Id,
                ScreenId = show.ScreenId,
                MovieId = show.MovieId,
                ShowDate = show.ShowDate,
                ShowTime = show.ShowTime,
                Language = show.Language,
                Format = show.Format,
                Status = show.Status,
                Prices = prices.Select(p => new ShowSeatPriceDto
                {
                    SeatCategoryId = p.SeatCategoryId,
                    Price = p.Price
                }).ToList()
            };
            
            // Populate category names
            using var connection = _connectionFactory.CreateConnection();
            foreach (var p in dto.Prices)
            {
                p.CategoryName = await connection.QuerySingleOrDefaultAsync<string>(
                    "SELECT CategoryName FROM SeatCategories WHERE Id = @Id", new { Id = p.SeatCategoryId }) ?? "";
            }

            return dto;
        }

        public async Task<bool> CreateShowAsync(CreateShowDto request)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var show = new Shows
                {
                    ScreenId = request.ScreenId,
                    MovieId = request.MovieId,
                    ShowDate = request.ShowDate,
                    ShowTime = request.ShowTime,
                    Language = request.Language,
                    Format = request.Format,
                    Status = request.Status,
                    CreatedAt = DateTime.UtcNow
                };
                
                int showId = await _showRepository.CreateShowAsync(connection, transaction, show);
                
                foreach (var p in request.Prices)
                {
                    await _showRepository.CreateShowSeatPriceAsync(connection, transaction, new ShowSeatPrices
                    {
                        ShowId = showId,
                        SeatCategoryId = p.SeatCategoryId,
                        Price = p.Price,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        public async Task<bool> UpdateShowAsync(CreateShowDto request)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var show = await _showRepository.GetShowByIdAsync(request.Id);
                if (show == null) return false;
                
                show.ScreenId = request.ScreenId;
                show.MovieId = request.MovieId;
                show.ShowDate = request.ShowDate;
                show.ShowTime = request.ShowTime;
                show.Language = request.Language;
                show.Format = request.Format;
                show.Status = request.Status;
                show.UpdatedAt = DateTime.UtcNow;
                
                await _showRepository.UpdateShowAsync(connection, transaction, show);
                
                // Recreate prices safely
                await _showRepository.DeleteShowSeatPricesAsync(connection, transaction, show.Id);
                
                foreach (var p in request.Prices)
                {
                    await _showRepository.CreateShowSeatPriceAsync(connection, transaction, new ShowSeatPrices
                    {
                        ShowId = show.Id,
                        SeatCategoryId = p.SeatCategoryId,
                        Price = p.Price,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        public async Task<bool> DeleteShowAsync(int showId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                await _showRepository.DeleteShowSeatPricesAsync(connection, transaction, showId);
                await _showRepository.DeleteShowAsync(connection, transaction, showId);
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        public async Task<List<ShowSeatPriceDto>> GetDefaultSeatPricesForScreenAsync(int screenId)
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
                SELECT DISTINCT sc.Id AS SeatCategoryId, sc.CategoryName
                FROM ScreenRows sr
                INNER JOIN SeatCategories sc ON sr.SeatCategoryId = sc.Id
                WHERE sr.ScreenId = @ScreenId AND sr.SeatCategoryId IS NOT NULL;";
                
            var categories = await connection.QueryAsync<dynamic>(sql, new { ScreenId = screenId });
            
            return categories.Select(c => new ShowSeatPriceDto
            {
                SeatCategoryId = c.SeatCategoryId,
                CategoryName = c.CategoryName,
                Price = 0 // Default 0
            }).ToList();
        }
    }
}
