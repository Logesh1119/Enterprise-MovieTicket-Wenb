using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.Interfaces.Booking;
using MovieTicketBooking.Application.DTOs.Customer.Booking;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieTicketBooking.Pages.Customer.Profile
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IBookingService _bookingService;

        public IndexModel(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IEnumerable<BookingSummaryDto> RecentBookings { get; set; } = new List<BookingSummaryDto>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Challenge();
            
            int userId = int.Parse(userIdClaim.Value);
            Username = User.Identity?.Name ?? "User";
            
            // Sometimes email is stored in Email claim or Name claim
            var emailClaim = User.FindFirst(ClaimTypes.Email);
            Email = emailClaim?.Value ?? "Not provided";

            // Fetch bookings
            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            
            // Just take the top 3 for the profile overview
            var bookingsList = new List<BookingSummaryDto>();
            int count = 0;
            foreach (var b in bookings)
            {
                if (count >= 3) break;
                bookingsList.Add(b);
                count++;
            }
            RecentBookings = bookingsList;

            return Page();
        }
    }
}
