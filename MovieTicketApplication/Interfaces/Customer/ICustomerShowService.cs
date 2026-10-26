using System.Threading.Tasks;
using MovieTicketBooking.Application.DTOs.Customer.Show;

namespace MovieTicketBooking.Application.Interfaces.Customer
{
    public interface ICustomerShowService
    {
        Task<CustomerSeatLayoutDto> GetShowSeatLayoutAsync(int showId);
    }
}
