using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.DTOs.Customer.Show;
using MovieTicketBooking.Application.Interfaces.Customer;
using Microsoft.AspNetCore.Authorization;

namespace MovieTicketBooking.Pages.Customer.Shows
{
    [Authorize]
    public class SeatSelectionModel : PageModel
    {
        private readonly ICustomerShowService _showService;

        public SeatSelectionModel(ICustomerShowService showService)
        {
            _showService = showService;
        }

        public CustomerSeatLayoutDto Layout { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int Qty { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Set default quantity if someone tries to bypass
            if (Qty < 1) Qty = 1;
            if (Qty > 10) Qty = 10;

            Layout = await _showService.GetShowSeatLayoutAsync(id);
            if (Layout == null) return NotFound();

            return Page();
        }
    }
}
