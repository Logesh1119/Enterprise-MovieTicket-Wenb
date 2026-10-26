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
    public class ConfirmationModel : PageModel
    {
        private readonly IBookingService _bookingService;

        public ConfirmationModel(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public BookingSummaryDto Summary { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Challenge();
            int userId = int.Parse(userIdClaim.Value);

            Summary = await _bookingService.GetBookingSummaryAsync(id, userId);
            
            if (Summary == null) return NotFound();

            return Page();
        }
    }
}
