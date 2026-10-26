using Dapper;
using Microsoft.AspNetCore.Connections;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBooking.Domain.Entities;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Theater;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.TheaterCreation;
using MovieTicketBookingDomain.Entities.TheaterAdmin;
using MovieTicketBookingInfrastructureData;
using System.Data;
using System.Security.Claims;

namespace MovieTicketBooking.Infrastructure.Repositories;

public class TheaterRepository : ITheaterRepository
{

    private readonly SqlConnectionFactory _connectionFactory;
    private readonly IMediaRepository _mediaRepository;
    public TheaterRepository(SqlConnectionFactory connectionFactory, IMediaRepository mediaRepository)
    {
        _connectionFactory = connectionFactory;
        _mediaRepository = mediaRepository;
    }

    public async Task<int> CreateTheaterAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        Theater theater)
    {
        string sql = @"
        INSERT INTO Theaters
        (
            OwnerUserId,
            TheaterName,
            TheaterCode,
            TheaterBrand,
            TheaterDescription,
            Status
        )
        VALUES
        (
            @OwnerUserId,
            @TheaterName,
            @TheaterCode,
            @TheaterBrand,
            @TheaterDescription,
            @Status
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);";

        return await connection.ExecuteScalarAsync<int>(
            sql,
            theater,
            transaction);
    }

    public async Task CreateTheaterInformationAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        TheaterInformation information)
    {
        string sql = @"
        INSERT INTO TheaterInformation
        (
            TheaterId,
            AddressLine1,
            AddressLine2,
            Area,
            City,
            State,
            Country,
            ZipCode,
            ContactName,
            ContactPhone
        )
        VALUES
        (
            @TheaterId,
            @AddressLine1,
            @AddressLine2,
            @Area,
            @City,
            @State,
            @Country,
            @ZipCode,
            @ContactName,
            @ContactPhone
        );";

        await connection.ExecuteAsync(
            sql,
            information,
            transaction);
    }

    public async Task AddTheaterImageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        TheaterImage image)
    {
        string sql = @"
        INSERT INTO TheaterImages
        (
            TheaterId,
            MediaId,
            DisplayOrder,
            IsPrimary
        )
        VALUES
        (
            @TheaterId,
            @MediaId,
            @DisplayOrder,
            @IsPrimary
        );";

        await connection.ExecuteAsync(
            sql,
            image,
            transaction);
    }

    public async Task<List<TheaterCardDto>> GetTheatersAsync(int? ownerUserId = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        string sql = @"
SELECT
    t.Id AS TheaterId,
    t.OwnerUserId,
    t.TheaterName,
    t.TheaterBrand,
    CONCAT(ti.AddressLine1, ', ', ti.Area, ', ', ti.City, ', ', ti.State, ' ', ti.ZipCode) AS Address,
    m.FilePath AS ImagePath,
    t.Status,
    t.TheaterCode,
    t.TheaterDescription AS Description,
    ti.AddressLine1,
    ti.AddressLine2,
    ti.Area,
    ti.City,
    ti.State,
    ti.Country,
    ti.ZipCode,
    ti.ContactName,
    ti.ContactPhone,
    (
        SELECT STRING_AGG(a.AmenityName, ',')
        FROM (
            SELECT DISTINCT am2.AmenityName
            FROM ScreenAmenities sa2
            JOIN Amenities am2 ON am2.Id = sa2.AmenityId
            JOIN Screens s2 ON s2.Id = sa2.ScreenId
            WHERE s2.TheaterId = t.Id
        ) a
    ) AS AmenitiesString
FROM Theaters t
INNER JOIN TheaterInformation ti ON ti.TheaterId = t.Id
LEFT JOIN TheaterImages img ON img.TheaterId = t.Id AND img.IsPrimary = 1
LEFT JOIN Media m ON m.Id = img.MediaId
WHERE (@ownerUserId IS NULL OR t.OwnerUserId = @ownerUserId)
ORDER BY t.TheaterName;";

        var theaters = (await connection.QueryAsync<TheaterCardDto>(
            sql,
            new { ownerUserId })).ToList();

        foreach (var theater in theaters)
        {
            theater.Amenities = string.IsNullOrWhiteSpace(theater.AmenitiesString)
                ? new List<string>()
                : theater.AmenitiesString
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Distinct()
                    .ToList();
        }

        return theaters;
    }

    public async Task<CreateTheater?> GetTheaterByIdAsync(
        int theaterId,
        int? ownerUserId = null)
    {
        const string sql = @"
SELECT
    t.Id AS TheaterId,
    t.OwnerUserId,
    t.TheaterName,
    t.TheaterCode,
    t.TheaterBrand,
    t.TheaterDescription,
    t.Status,

    ti.AddressLine1,
    ti.AddressLine2,
    ti.Area,
    ti.City,
    ti.State,
    ti.Country,
    ti.ZipCode,
    ti.ContactName,
    ti.ContactPhone
FROM Theaters t
INNER JOIN TheaterInformation ti
    ON ti.TheaterId = t.Id
WHERE
    t.Id = @TheaterId
AND (@OwnerUserId IS NULL OR t.OwnerUserId = @OwnerUserId);";

        using var connection = _connectionFactory.CreateConnection();

        var theater = await connection.QueryFirstOrDefaultAsync<CreateTheater>(
            sql,
            new
            {
                TheaterId = theaterId,
                OwnerUserId = ownerUserId
            });

        if (theater == null)
            return null;

        const string imageSql = @"
SELECT
    m.Id AS MediaId,
    m.FilePath,
    ti.IsPrimary
FROM TheaterImages ti
INNER JOIN Media m
    ON m.Id = ti.MediaId
WHERE ti.TheaterId = @TheaterId
ORDER BY ti.DisplayOrder;";

        theater.ExistingImages = (
            await connection.QueryAsync<ExistingImageDto>(
                imageSql,
                new { TheaterId = theaterId }))
            .ToList();

        return theater;
    }

    public async Task<bool> UpdateTheaterAsync(
    CreateTheater theater,
    int? ownerUserId = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            string theaterSql = @"
UPDATE Theaters
SET
    OwnerUserId = CASE WHEN @NewOwnerUserId > 0 THEN @NewOwnerUserId ELSE OwnerUserId END,
    TheaterName = @TheaterName,
    TheaterCode = @TheaterCode,
    TheaterBrand = @TheaterBrand,
    TheaterDescription = @TheaterDescription,
    Status = @Status
WHERE
    Id = @TheaterId
AND (@OwnerUserId IS NULL OR OwnerUserId = @OwnerUserId);";

            int theaterRows = await connection.ExecuteAsync(
                theaterSql,
                new
                {
                    theater.TheaterId,
                    OwnerUserId = ownerUserId, // Used for WHERE clause
                    NewOwnerUserId = theater.OwnerUserId, // From the DTO, used for SET
                    theater.TheaterName,
                    theater.TheaterCode,
                    theater.TheaterBrand,
                    theater.TheaterDescription,
                    theater.Status
                },
                transaction);

            if (theaterRows == 0)
            {
                transaction.Rollback();
                return false;
            }

            string infoSql = @"
UPDATE TheaterInformation
SET
    AddressLine1 = @AddressLine1,
    AddressLine2 = @AddressLine2,
    Area = @Area,
    City = @City,
    State = @State,
    Country = @Country,
    ZipCode = @ZipCode,
    ContactName = @ContactName,
    ContactPhone = @ContactPhone
WHERE TheaterId = @TheaterId;";

            await connection.ExecuteAsync(
                infoSql,
                new
                {
                    theater.TheaterId,
                    theater.AddressLine1,
                    theater.AddressLine2,
                    theater.Area,
                    theater.City,
                    theater.State,
                    theater.Country,
                    theater.ZipCode,
                    theater.ContactName,
                    theater.ContactPhone
                },
                transaction);

            if (theater.DeletedMediaIds != null &&
    theater.DeletedMediaIds.Any())
            {
                const string deleteTheaterImagesSql = @"
DELETE FROM TheaterImages
WHERE
    TheaterId = @TheaterId
AND MediaId = @MediaId;";

                foreach (int mediaId in theater.DeletedMediaIds)
                {
                    await connection.ExecuteAsync(
                        deleteTheaterImagesSql,
                        new
                        {
                            theater.TheaterId,
                            MediaId = mediaId
                        },
                        transaction);

                    await connection.ExecuteAsync(
                        "DELETE FROM Media WHERE Id=@Id",
                        new { Id = mediaId },
                        transaction);
                }
            }

            if (theater.Images != null &&
    theater.Images.Any())
            {
                int displayOrder =
                    await connection.ExecuteScalarAsync<int>(
                        @"SELECT ISNULL(MAX(DisplayOrder),0)
              FROM TheaterImages
              WHERE TheaterId=@TheaterId",
                        new { theater.TheaterId },
                        transaction);

                foreach (var image in theater.Images)
                {
                    displayOrder++;

                    Media media =
                        await _mediaRepository.SaveImageAsync(
                            image,
                            "theaters");

                    int mediaId =
                        await _mediaRepository.CreateAsync(
                            connection,
                            transaction,
                            media);

                    await AddTheaterImageAsync(
                        connection,
                        transaction,
                        new TheaterImage
                        {
                            TheaterId = theater.TheaterId,
                            MediaId = mediaId,
                            DisplayOrder = displayOrder,
                            IsPrimary = displayOrder == 1
                        });
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