using Microsoft.AspNetCore.Http;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBooking.Domain.Entities;
using MovieTicketBooking.Infrastructure.Repositories;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Theater;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.TheaterCreation;
using MovieTicketBookingDomain.Entities.TheaterAdmin;
using MovieTicketBookingInfrastructureData;
using System.Security.Claims;

namespace MovieTicketBookingInfrastructure.Service.TheaterAdmin
{
    public class TheaterService : ITheaterService
    {
        private readonly ITheaterRepository _repository;
        private readonly IMediaRepository _mediaRepository;
        private readonly SqlConnectionFactory _connectionFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TheaterService(ITheaterRepository repository, IMediaRepository mediaRepository, SqlConnectionFactory connectionFactory, IHttpContextAccessor httpContext)
        {
            _repository = repository;
            _mediaRepository = mediaRepository;
            _connectionFactory = connectionFactory;
            _httpContextAccessor = httpContext;
        }

        private int? GetOwnerUserIdForQuery()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity!.IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated.");

            if (user.IsInRole("SuperAdmin"))
                return null; // Global access

            string? userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int theaterAdminId))
                throw new UnauthorizedAccessException("Invalid User.");

            return theaterAdminId;
        }

        public async Task<bool> CreateTheaterAsync(CreateTheater request)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity!.IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated.");

            int ownerUserId;

            if (user.IsInRole("SuperAdmin"))
            {
                // SuperAdmin must specify which TheaterAdmin owns this theater
                // request.OwnerUserId needs to be passed, but for now if it's 0, we can use their own ID as fallback
                ownerUserId = request.OwnerUserId > 0 ? request.OwnerUserId : int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            }
            else
            {
                string? userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out ownerUserId))
                    throw new UnauthorizedAccessException("Invalid User.");
            }

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var theater = new Theater
                {
                    OwnerUserId = ownerUserId,
                    TheaterName = request.TheaterName,
                    TheaterCode = request.TheaterCode,
                    TheaterBrand = request.TheaterBrand,
                    TheaterDescription = request.TheaterDescription,
                    Status = request.Status
                };

                int theaterId = await _repository.CreateTheaterAsync(connection, transaction, theater);

                var theaterinformation = new TheaterInformation
                {
                    TheaterId = theaterId,
                    AddressLine1 = request.AddressLine1,
                    AddressLine2 = request.AddressLine2,
                    Area = request.Area,
                    City = request.City,
                    State = request.State,
                    Country = request.Country,
                    ZipCode = request.ZipCode,
                    ContactName = request.ContactName,
                    ContactPhone = request.ContactPhone,
                };

                await _repository.CreateTheaterInformationAsync(connection, transaction, theaterinformation);

                if (request.Images != null && request.Images.Any())
                {
                    int displayOrder = 1;
                    foreach (var image in request.Images)
                    {
                        Media media = await _mediaRepository.SaveImageAsync(image, "theaters");
                        int mediaId = await _mediaRepository.CreateAsync(connection, transaction, media);

                        await _repository.AddTheaterImageAsync(
                                connection, transaction,
                                new TheaterImage
                                {
                                    TheaterId = theaterId,
                                    MediaId = mediaId,
                                    DisplayOrder = displayOrder,
                                    IsPrimary = displayOrder == 1,
                                });
                        displayOrder++;
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

        public async Task<List<TheaterCardDto>> GetTheatersAsync()
        {
            int? ownerUserId = GetOwnerUserIdForQuery();
            return await _repository.GetTheatersAsync(ownerUserId);
        }

        public async Task<CreateTheater?> GetTheaterByIdAsync(int theaterId)
        {
            int? ownerUserId = GetOwnerUserIdForQuery();
            return await _repository.GetTheaterByIdAsync(theaterId, ownerUserId);
        }

        public async Task<bool> UpdateTheaterAsync(CreateTheater theater)
        {
            int? ownerUserId = GetOwnerUserIdForQuery();
            // If super admin is editing, we should probably not change the OwnerUserId or we might need a separate mechanism.
            // For now, passing null to repository means the query shouldn't filter by ownerUserId.
            return await _repository.UpdateTheaterAsync(theater, ownerUserId);
        }

    }

}

