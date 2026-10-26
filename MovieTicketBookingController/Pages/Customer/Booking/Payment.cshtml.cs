using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.DTOs.Customer.Booking;
using MovieTicketBooking.Application.Interfaces.Booking;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBooking.Application.Interfaces.Customer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace MovieTicketBooking.Pages.Customer.Booking
{
    [Authorize]
    public class PaymentModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly ICustomerShowService _showService;
        private readonly IEmailService _emailService;
        private readonly ILogger<PaymentModel> _logger;

        public PaymentModel(
            IBookingService bookingService, 
            ICustomerShowService showService,
            IEmailService emailService, 
            ILogger<PaymentModel> logger)
        {
            _bookingService = bookingService;
            _showService = showService;
            _emailService = emailService;
            _logger = logger;
        }

        [BindProperty]
        public int ShowId { get; set; }

        [BindProperty]
        public string SeatIds { get; set; }

        [BindProperty]
        public decimal TotalAmount { get; set; }

        [BindProperty]
        public string UpiId { get; set; }

        [BindProperty]
        public string CardNumber { get; set; }

        [BindProperty]
        public string CardExpiry { get; set; }

        [BindProperty]
        public string CardCvv { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ShowId == 0 || string.IsNullOrEmpty(SeatIds))
            {
                return RedirectToPage("/Customer/Home/Index");
            }

            var idList = SeatIds.Split(',').Select(int.Parse).ToList();
            var showDetails = await _showService.GetShowSeatLayoutAsync(ShowId);
            if (showDetails == null) return NotFound();

            TotalAmount = 0;
            foreach (var row in showDetails.Rows)
            {
                foreach (var seat in row.Seats)
                {
                    if (idList.Contains(seat.SeatId))
                    {
                        TotalAmount += seat.Price;
                    }
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostProcessPaymentAsync()
        {
            if (ShowId == 0 || string.IsNullOrEmpty(SeatIds))
            {
                return RedirectToPage("/Customer/Home/Index");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var emailClaim = User.FindFirst(ClaimTypes.Email);
            if (userIdClaim == null) return Challenge();
            
            int userId = int.Parse(userIdClaim.Value);
            string userEmail = emailClaim?.Value;
            var idList = SeatIds.Split(',').Select(int.Parse).ToList();

            var request = new CreateBookingDto
            {
                ShowId = ShowId,
                ScreenSeatIds = idList
            };

            try
            {
                // 1. Create Booking (Pending)
                int bookingId = await _bookingService.CreateBookingAsync(userId, request);
                
                // 2. Process Payment (Sandbox simulation)
                // Assuming successful payment...
                
                // 3. Update to Confirmed
                await _bookingService.UpdateBookingStatusAsync(bookingId, userId, "Confirmed");

                // 4. Send Confirmation Email
                if (!string.IsNullOrEmpty(userEmail))
                {
                    try
                    {
                        var summary = await _bookingService.GetBookingSummaryAsync(bookingId, userId);
                        string emailBody = $@"
                            <div style='font-family: Arial, sans-serif; padding: 20px; color: #333;'>
                                <h2>Booking Confirmed!</h2>
                                <p>Your booking for <strong>{summary.MovieName}</strong> is confirmed.</p>
                                <ul>
                                    <li><strong>Theater:</strong> {summary.TheaterName}</li>
                                    <li><strong>Screen:</strong> {summary.ScreenName}</li>
                                    <li><strong>Date & Time:</strong> {summary.ShowDate:MMM dd, yyyy} at {summary.ShowTime:hh\:mm}</li>
                                    <li><strong>Seats:</strong> {string.Join(", ", summary.SeatLabels)}</li>
                                    <li><strong>Amount Paid:</strong> &#8377;{summary.TotalAmount:0.00}</li>
                                </ul>
                                <p>Your Booking ID is: <strong>#BK-{bookingId}</strong></p>
                                <p>Thank you for choosing Movie Ticket Booking!</p>
                            </div>";
                        
                        await _emailService.SendEmailAsync(userEmail, "Your Booking Confirmation - BK-" + bookingId, emailBody);
                        TempData["EmailSent"] = true;
                    }
                    catch (Exception emailEx)
                    {
                        _logger.LogError(emailEx, "Failed to send booking confirmation email to {Email}", userEmail);
                        TempData["EmailSent"] = false;
                    }
                }

                return RedirectToPage("/Customer/Booking/Confirmation", new { id = bookingId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment processing failed.");
                ModelState.AddModelError("", ex.Message);
                return await OnPostAsync();
            }
        }
    }
}
