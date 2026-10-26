using System.Collections.Generic;
using System.Threading.Tasks;
using MovieTicketBooking.Application.DTOs.Customer.Booking;

namespace MovieTicketBooking.Application.Interfaces.Booking
{
    public interface IBookingService
    {
        Task<int> CreateBookingAsync(int userId, CreateBookingDto request);
        Task<BookingSummaryDto> GetBookingSummaryAsync(int bookingId, int userId);
        Task<IEnumerable<BookingSummaryDto>> GetUserBookingsAsync(int userId);
        Task<bool> UpdateBookingStatusAsync(int bookingId, int userId, string status);
    }
}
