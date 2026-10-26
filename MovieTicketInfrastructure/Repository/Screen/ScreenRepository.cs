using Dapper;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen;
using MovieTicketBookingApplication.Interfaces.Screen;
using MovieTicketBookingDomain.Entities.Screen;
using MovieTicketBookingDomain.Entities.Screens;
using MovieTicketBookingInfrastructureData;
using System.Data;


namespace MovieTicketBookingInfrastructure.Repository.Screen
{
    public class ScreenRepository : IScreenRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public ScreenRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task CreateAmenityAsync(IDbConnection connection, IDbTransaction transaction, Amenities amenities)
        {
            string sql = @"
        INSERT INTO ScreenAmenities
        (
            ScreenId,
            AmenityId
        )
        VALUES
        (
            @ScreenId,
            @AmenityId
        );";

            await connection.ExecuteAsync(
                sql,
                amenities,
                transaction);
        }

        public async Task<int> CreateScreenAsync(IDbConnection connection, IDbTransaction transaction, Screens screens)
        {
            string sql = @"
        INSERT INTO Screens
        (
            TheaterId,
            ScreenName,
            ScreenCode,
            Floor,
            ScreenType,
            AudioTechnology,
            SeatingType,
            Status
        )
        VALUES
        (
            @TheaterId,
            @ScreenName,
            @ScreenCode,
            @Floor,
            @ScreenType,
            @AudioTechnology,
            @SeatingType,
            @Status
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await connection.ExecuteScalarAsync<int>(sql, screens, transaction);
        }

        public async Task<List<CreateAmenity>> GetAmenityAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @" SELECT Id,AmenityName FROM Amenities ORDER BY AmenityName;";
            var result = await connection.QueryAsync<CreateAmenity>(sql);

            return result.ToList();
        }

        public async Task<int> GetTheaterIdByUserIdAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"SELECT Id FROM Theaters WHERE OwnerUserId = @UserId;";
            return await connection.ExecuteScalarAsync<int>(
                sql,
                new { UserId = userId });
        }

        public async Task<int> CreateScreenRowAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    ScreenRows screenRow)
        {
            string sql = @"
    INSERT INTO ScreenRows
    (
        ScreenId,
        RowName,
        RowType,
        TotalSeats,
        WalkwayCount,
        SeatCategoryId,
        DisplayOrder,
        IsReverse
    )
    VALUES
    (
        @ScreenId,
        @RowName,
        @RowType,
        @TotalSeats,
        @WalkwayCount,
        @SeatCategoryId,
        @DisplayOrder,
        @IsReverse
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await connection.ExecuteScalarAsync<int>(
                sql,
                screenRow,
                transaction);
        }

        public async Task CreateScreenSeatAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    ScreenSeats screenSeat)
        {
            string sql = @"
    INSERT INTO ScreenSeats
    (
        ScreenRowId,
        SeatNo,
        SeatLabel,
        SeatType,
        Status
    )
    VALUES
    (
        @ScreenRowId,
        @SeatNo,
        @SeatLabel,
        @SeatType,
        @Status
    );";

            await connection.ExecuteAsync(
                sql,
                screenSeat,
                transaction);
        }


        public async Task<List<ScreenRows>> GetScreenRowsByScreenIdAsync(int screenId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
        SELECT *
        FROM ScreenRows
        WHERE ScreenId = @ScreenId
        ORDER BY DisplayOrder;";

            var result = await connection.QueryAsync<ScreenRows>(
                sql,
                new { ScreenId = screenId });

            return result.ToList();
        }


        public async Task<List<ScreenSeats>> GetScreenSeatsByRowIdAsync(int screenRowId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
        SELECT *
        FROM ScreenSeats
        WHERE ScreenRowId = @ScreenRowId
        ORDER BY SeatNo;";

            var result = await connection.QueryAsync<ScreenSeats>(
                sql,
                new { ScreenRowId = screenRowId });

            return result.ToList();
        }

        public async Task<List<SeatCategories>> GetSeatCategoriesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            string sql = @"
        SELECT *
        FROM SeatCategories
        ORDER BY CategoryName;";

            var result = await connection.QueryAsync<SeatCategories>(sql);

            return result.ToList();
        }


        public async Task DeleteScreenSeatsAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    int screenId)
        {
            string sql = @"
DELETE SS
FROM ScreenSeats SS
INNER JOIN ScreenRows SR
ON SR.Id = SS.ScreenRowId
WHERE SR.ScreenId = @ScreenId;";

            await connection.ExecuteAsync(
                sql,
                new { ScreenId = screenId },
                transaction);
        }

        public async Task DeleteScreenRowsAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    int screenId)
        {
            string sql = @"
DELETE FROM ScreenRows
WHERE ScreenId=@ScreenId;";

            await connection.ExecuteAsync(
                sql,
                new { ScreenId = screenId },
                transaction);
        }

        public async Task<bool> SaveSeatAllocationAsync(
            List<SeatAllocationDto> request)
        {
            using var connection = _connectionFactory.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                string sql = @"
UPDATE ScreenRows
SET SeatCategoryId=@SeatCategoryId
WHERE Id=@ScreenRowId;";

                foreach (var row in request)
                {
                    await connection.ExecuteAsync(
                        sql,
                        row,
                        transaction);
                }

                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<List<ScreenCardDto>> GetScreensByTheaterIdAsync(int theaterId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
SELECT 
    s.Id AS ScreenId,
    s.TheaterId,
    s.ScreenName,
    s.ScreenCode,
    s.Floor,
    s.ScreenType,
    s.AudioTechnology,
    s.SeatingType,
    s.Status,
    (SELECT COUNT(Id) FROM ScreenRows WHERE ScreenId = s.Id) AS TotalRows,
    (SELECT COUNT(ss.Id) FROM ScreenSeats ss JOIN ScreenRows sr ON sr.Id = ss.ScreenRowId WHERE sr.ScreenId = s.Id AND ss.SeatType = 'Seat') AS TotalSeats,
    (SELECT STRING_AGG(a.AmenityName, ',') FROM ScreenAmenities sa JOIN Amenities a ON a.Id = sa.AmenityId WHERE sa.ScreenId = s.Id) AS AmenitiesString
FROM Screens s
WHERE s.TheaterId = @TheaterId
ORDER BY s.ScreenName;";

            var screens = (await connection.QueryAsync<ScreenCardDto>(sql, new { TheaterId = theaterId })).ToList();

            foreach (var sc in screens)
            {
                sc.Amenities = string.IsNullOrWhiteSpace(sc.AmenitiesString)
                    ? new List<string>()
                    : sc.AmenitiesString.Split(',', StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
            }

            return screens;
        }

        public async Task<CreateScreen?> GetScreenByIdAsync(int screenId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = @"
SELECT 
    Id AS ScreenId,
    TheaterId,
    ScreenName,
    ScreenCode,
    Floor,
    ScreenType,
    AudioTechnology,
    SeatingType,
    Status
FROM Screens
WHERE Id = @ScreenId;";

            var screen = await connection.QueryFirstOrDefaultAsync<CreateScreen>(sql, new { ScreenId = screenId });
            if (screen == null) return null;

            string amenitySql = @"SELECT AmenityId FROM ScreenAmenities WHERE ScreenId = @ScreenId;";
            screen.AmenId = (await connection.QueryAsync<int>(amenitySql, new { ScreenId = screenId })).ToList();

            return screen;
        }

        public async Task<bool> UpdateScreenAsync(int screenId, CreateScreen request)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                string sql = @"
UPDATE Screens
SET 
    ScreenName = @ScreenName,
    ScreenCode = @ScreenCode,
    Floor = @Floor,
    ScreenType = @ScreenType,
    AudioTechnology = @AudioTechnology,
    SeatingType = @SeatingType,
    Status = @Status
WHERE Id = @ScreenId;";

                await connection.ExecuteAsync(sql, new
                {
                    ScreenId = screenId,
                    request.ScreenName,
                    request.ScreenCode,
                    request.Floor,
                    request.ScreenType,
                    request.AudioTechnology,
                    request.SeatingType,
                    request.Status
                }, transaction);

                // Update amenities
                string delAmenSql = @"DELETE FROM ScreenAmenities WHERE ScreenId = @ScreenId;";
                await connection.ExecuteAsync(delAmenSql, new { ScreenId = screenId }, transaction);

                if (request.AmenId != null)
                {
                    foreach (int amenId in request.AmenId)
                    {
                        var screenAmenity = new Amenities
                        {
                            ScreenId = screenId,
                            AmenityId = amenId
                        };
                        await CreateAmenityAsync(connection, transaction, screenAmenity);
                    }
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}

