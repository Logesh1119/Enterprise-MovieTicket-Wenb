using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.DTOs.Customer.Theater;
using MovieTicketBooking.Application.Interfaces.Customer;

namespace MovieTicketBooking.Pages.Customer.Theaters
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerTheaterService _theaterService;

        public IndexModel(ICustomerTheaterService theaterService)
        {
            _theaterService = theaterService;
        }

        public IEnumerable<CustomerTheaterDto> Theaters { get; set; } = new List<CustomerTheaterDto>();

        public async Task OnGetAsync()
        {
            Theaters = await _theaterService.GetAllTheatersAsync();
        }
    }
}
