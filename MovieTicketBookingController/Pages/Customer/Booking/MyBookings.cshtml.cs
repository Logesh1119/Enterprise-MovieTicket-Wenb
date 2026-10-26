using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.DTOs.Customer.Booking;
using MovieTicketBooking.Application.Interfaces.Booking;
using System.Security.Claims;

namespace MovieTicketBooking.Pages.Customer.Booking
{
    [Authorize]
    public class MyBookingsModel : PageModel
    {
        private readonly IBookingService _bookingService;

        public MyBookingsModel(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public IEnumerable<BookingSummaryDto> Bookings { get; set; } = new List<BookingSummaryDto>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Challenge();
            int userId = int.Parse(userIdClaim.Value);

            Bookings = await _bookingService.GetUserBookingsAsync(userId);
            
            return Page();
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Challenge();
            int userId = int.Parse(userIdClaim.Value);

            // Update status to 'Cancelled'
            await _bookingService.UpdateBookingStatusAsync(id, userId, "Cancelled");

            // Redirect to refresh the page
            TempData["Message"] = "Booking cancelled successfully.";
            return RedirectToPage();
        }
    }
}
