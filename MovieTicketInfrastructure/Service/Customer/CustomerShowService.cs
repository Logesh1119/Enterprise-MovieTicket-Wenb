using System.Threading.Tasks;
using MovieTicketBooking.Application.DTOs.Customer.Show;
using MovieTicketBooking.Application.Interfaces.Customer;

namespace MovieTicketBooking.Infrastructure.Service.Customer
{
    public class CustomerShowService : ICustomerShowService
    {
        private readonly ICustomerShowRepository _repository;

        public CustomerShowService(ICustomerShowRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomerSeatLayoutDto> GetShowSeatLayoutAsync(int showId)
        {
            return await _repository.GetShowSeatLayoutAsync(showId);
        }
    }
}
