using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.DTOs.Customer.Booking;
using MovieTicketBooking.Application.DTOs.Customer.Show;
using MovieTicketBooking.Application.Interfaces.Customer;
using MovieTicketBooking.Application.Interfaces.Booking;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MovieTicketBooking.Pages.Customer.Booking
{
    [Authorize]
    public class SummaryModel : PageModel
    {
        private readonly ICustomerShowService _showService;
        private readonly IBookingService _bookingService;

        public SummaryModel(ICustomerShowService showService, IBookingService bookingService)
        {
            _showService = showService;
            _bookingService = bookingService;
        }

        public CustomerSeatLayoutDto ShowDetails { get; set; }
        public List<CustomerSeatDto> SelectedSeats { get; set; } = new List<CustomerSeatDto>();
        public decimal TotalAmount { get; set; }

        [BindProperty]
        public int ShowId { get; set; }

        [BindProperty]
        public string SeatIds { get; set; } // Comma separated

        public async Task<IActionResult> OnPostAsync()
        {
            if (ShowId == 0 || string.IsNullOrEmpty(SeatIds))
            {
                return RedirectToPage("/Customer/Home/Index");
            }

            var idList = SeatIds.Split(',').Select(int.Parse).ToList();
            
            ShowDetails = await _showService.GetShowSeatLayoutAsync(ShowId);
            if (ShowDetails == null) return NotFound();

            foreach (var row in ShowDetails.Rows)
            {
                foreach (var seat in row.Seats)
                {
                    if (idList.Contains(seat.SeatId))
                    {
                        if (seat.IsBooked)
                        {
                            ModelState.AddModelError("", $"Seat {seat.SeatLabel} is already booked.");
                            return Page(); // Cannot book
                        }
                        SelectedSeats.Add(seat);
                        TotalAmount += seat.Price;
                    }
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostConfirmAsync()
        {
            if (ShowId == 0 || string.IsNullOrEmpty(SeatIds))
            {
                return RedirectToPage("/Customer/Home/Index");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Challenge();
            
            int userId = int.Parse(userIdClaim.Value);
            var idList = SeatIds.Split(',').Select(int.Parse).ToList();

            var request = new CreateBookingDto
            {
                ShowId = ShowId,
                ScreenSeatIds = idList
            };

            try
            {
                int bookingId = await _bookingService.CreateBookingAsync(userId, request);
                // Also update to Confirmed for dummy payment
                await _bookingService.UpdateBookingStatusAsync(bookingId, userId, "Confirmed");
                
                return RedirectToPage("/Customer/Booking/Confirmation", new { id = bookingId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                
                // Reload data to display error on same page
                ShowDetails = await _showService.GetShowSeatLayoutAsync(ShowId);
                foreach (var row in ShowDetails.Rows)
                {
                    foreach (var seat in row.Seats)
                    {
                        if (idList.Contains(seat.SeatId))
                        {
                            SelectedSeats.Add(seat);
                            TotalAmount += seat.Price;
                        }
                    }
                }
                return Page();
            }
        }
    }
}
