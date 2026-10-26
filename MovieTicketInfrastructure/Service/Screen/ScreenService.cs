using Microsoft.AspNetCore.Http;
using MovieTicketBooking.Domain.Entities;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen;
using MovieTicketBookingApplication.Interfaces.Screen;
using MovieTicketBookingDomain.Entities.Movies;
using MovieTicketBookingDomain.Entities.Screen;
using MovieTicketBookingDomain.Entities.Screens;
using MovieTicketBookingInfrastructure.Repository.Screen;
using MovieTicketBookingInfrastructureData;
using System.Security.Claims;

namespace MovieTicketBookingInfrastructure.Service.Screen
{
    public class ScreenService : IScreenService
    {
        private readonly IScreenRepository _repository;
        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ScreenService(IScreenRepository repository, SqlConnectionFactory connectionFactory, IHttpContextAccessor httpContext)
        {
            _repository = repository;
            _connectionFactory = connectionFactory;
            _httpContextAccessor = httpContext;
        }

        public async Task<int> CreateScreen(CreateScreen request, int userId)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity!.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            string? userIdClaim =
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out int theaterAdminId))
            {
                throw new UnauthorizedAccessException("Invalid User.");
            }

            using var connection = _connectionFactory.CreateConnection();


            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Step 1
                int theaterId =
                    await _repository.GetTheaterIdByUserIdAsync(userId);

                var screens = new Screens
                {
                    TheaterId = theaterId,

                    ScreenName = request.ScreenName,

                    ScreenCode = request.ScreenCode,

                    Floor = request.Floor,

                    ScreenType = request.ScreenType,

                    AudioTechnology = request.AudioTechnology,

                    SeatingType = request.SeatingType,

                    Status = request.Status,

                };

                int screenId = await _repository.CreateScreenAsync(connection, transaction, screens);

                foreach (int amenId in request.AmenId)
                {
                    var screenamenity = new Amenities
                    {
                        ScreenId = screenId,
                        AmenityId = amenId
                    };

                    await _repository.CreateAmenityAsync(
                        connection,
                        transaction,
                        screenamenity);
                }

                transaction.Commit();

                return screenId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<List<CreateAmenity>> GetAsync()
        {
            return await _repository.GetAmenityAsync();
        }

        public async Task<ScreenLayoutDto> SaveScreenLayoutAsync(ScreenLayoutDto request)
        {
            if (request.ScreenId <= 0)
                throw new Exception("Invalid Screen.");

            using var connection = _connectionFactory.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Validate SeatCategoryIds
                var validCategories = await _repository.GetSeatCategoriesAsync();
                var validCategoryIds = validCategories.Select(c => c.Id).ToList();

                foreach (var row in request.Rows)
                {
                    if (row.SeatCategoryId.HasValue && !validCategoryIds.Contains(row.SeatCategoryId.Value))
                    {
                        throw new Exception($"Invalid Seat Category: Category ID {row.SeatCategoryId.Value} does not exist.");
                    }
                }

                // Remove old layout
                await _repository.DeleteScreenSeatsAsync(
                    connection,
                    transaction,
                    request.ScreenId);

                await _repository.DeleteScreenRowsAsync(
                    connection,
                    transaction,
                    request.ScreenId);

                // Save new layout and capture each row's DB ID
                var savedLayout = new ScreenLayoutDto { ScreenId = request.ScreenId };

                foreach (var row in request.Rows)
                {
                    var screenRow = new ScreenRows
                    {
                        ScreenId = request.ScreenId,
                        RowName = row.RowName,
                        RowType = row.RowType,
                        TotalSeats = row.TotalSeats,
                        WalkwayCount = row.WalkwayCount,
                        SeatCategoryId = row.SeatCategoryId,
                        DisplayOrder = row.DisplayOrder,
                        IsReverse = row.IsReverse
                    };

                    int rowId = await _repository.CreateScreenRowAsync(
                        connection,
                        transaction,
                        screenRow);

                    foreach (var seat in row.Seats)
                    {
                        var screenSeat = new ScreenSeats
                        {
                            ScreenRowId = rowId,
                            SeatNo = seat.SeatNo,
                            SeatLabel = seat.SeatLabel,
                            SeatType = seat.SeatType,
                            Status = true
                        };

                        await _repository.CreateScreenSeatAsync(
                            connection,
                            transaction,
                            screenSeat);
                    }

                    // Build the saved row DTO with the real DB ID so JS can update its state
                    savedLayout.Rows.Add(new ScreenRowDto
                    {
                        Id = rowId,
                        RowName = row.RowName,
                        RowType = row.RowType,
                        TotalSeats = row.TotalSeats,
                        WalkwayCount = row.WalkwayCount,
                        SeatCategoryId = row.SeatCategoryId,
                        DisplayOrder = row.DisplayOrder,
                        IsReverse = row.IsReverse,
                        Seats = row.Seats
                    });
                }

                transaction.Commit();

                return savedLayout;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<ScreenLayoutDto?> GetScreenLayoutAsync(int screenId)
        {
            var rows =
                await _repository.GetScreenRowsByScreenIdAsync(screenId);

            if (!rows.Any())
                return null;

            var layout = new ScreenLayoutDto
            {
                ScreenId = screenId
            };

            foreach (var row in rows)
            {
                var seats =
                    await _repository.GetScreenSeatsByRowIdAsync(row.Id);

                layout.Rows.Add(new ScreenRowDto
                {
                    Id = row.Id,
                    RowName = row.RowName,
                    RowType = row.RowType,
                    TotalSeats = row.TotalSeats,
                    WalkwayCount = row.WalkwayCount,
                    SeatCategoryId = row.SeatCategoryId,
                    DisplayOrder = row.DisplayOrder,
                    IsReverse = row.IsReverse,

                    Seats = seats.Select(s => new ScreenSeatDto
                    {
                        SeatNo = s.SeatNo,
                        SeatLabel = s.SeatLabel,
                        SeatType = s.SeatType
                    }).ToList()
                });
            }

            return layout;
        }

        public async Task<bool> SaveSeatAllocationAsync(
            List<SeatAllocationDto> request)
        {
            return await _repository
                .SaveSeatAllocationAsync(request);
        }

        public async Task<List<SeatCategories>> GetSeatCategoriesAsync()
        {
            return await _repository.GetSeatCategoriesAsync();
        }

        public async Task<List<ScreenCardDto>> GetScreensByTheaterIdAsync(int theaterId)
        {
            return await _repository.GetScreensByTheaterIdAsync(theaterId);
        }

        public async Task<CreateScreen?> GetScreenByIdAsync(int screenId)
        {
            return await _repository.GetScreenByIdAsync(screenId);
        }

        public async Task<bool> UpdateScreenAsync(int screenId, CreateScreen request)
        {
            return await _repository.UpdateScreenAsync(screenId, request);
        }
    }
}

