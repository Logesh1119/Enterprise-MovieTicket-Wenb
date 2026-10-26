using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.DTOs.Customer.Show;
using MovieTicketBooking.Application.Interfaces.Customer;

namespace MovieTicketBooking.Pages.Customer.Shows
{
    public class TicketQuantityModel : PageModel
    {
        private readonly ICustomerShowService _showService;

        public TicketQuantityModel(ICustomerShowService showService)
        {
            _showService = showService;
        }

        public CustomerSeatLayoutDto ShowDetails { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ShowDetails = await _showService.GetShowSeatLayoutAsync(id);
            if (ShowDetails == null) return NotFound();

            return Page();
        }

        public IActionResult OnPost(int quantity)
        {
            if (quantity < 1 || quantity > 10)
            {
                ModelState.AddModelError("", "Please select a valid ticket quantity (1-10).");
                return RedirectToPage(new { id = Id });
            }

            // Redirect to SeatSelection, passing the quantity as a query parameter
            // Because SeatSelection will have [Authorize], this redirect will trigger the Login Gate if unauthenticated
            return RedirectToPage("/Customer/Shows/SeatSelection", new { id = Id, qty = quantity });
        }
    }
}
