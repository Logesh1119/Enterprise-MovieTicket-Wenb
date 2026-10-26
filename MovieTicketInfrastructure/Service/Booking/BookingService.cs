using System.Collections.Generic;
using System.Threading.Tasks;
using MovieTicketBooking.Application.DTOs.Customer.Booking;
using MovieTicketBooking.Application.Interfaces.Booking;

namespace MovieTicketBooking.Infrastructure.Service.Booking
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repository;

        public BookingService(IBookingRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> CreateBookingAsync(int userId, CreateBookingDto request)
        {
            return await _repository.CreateBookingAsync(userId, request);
        }

        public async Task<BookingSummaryDto> GetBookingSummaryAsync(int bookingId, int userId)
        {
            return await _repository.GetBookingSummaryAsync(bookingId, userId);
        }

        public async Task<IEnumerable<BookingSummaryDto>> GetUserBookingsAsync(int userId)
        {
            return await _repository.GetUserBookingsAsync(userId);
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, int userId, string status)
        {
            return await _repository.UpdateBookingStatusAsync(bookingId, userId, status);
        }
    }
}
