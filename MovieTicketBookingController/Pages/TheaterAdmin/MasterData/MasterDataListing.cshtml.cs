using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Movies;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.Screen;
using MovieTicketBookingApplication.DTO_s.TheaterAdmin.MasterData;
using System.Threading.Tasks;

namespace MovieTicketBooking.Pages.TheaterAdmin.MasterData
{
    [IgnoreAntiforgeryToken(Order = 1000)]
    public class MasterDataListingModel : PageModel
    {
        private readonly IMasterDataService _service;

        public MasterDataListingModel(IMasterDataService service)
        {
            _service = service;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnGetGenresAjaxAsync() => new JsonResult(await _service.GetGenresAsync());
        public async Task<IActionResult> OnPostCreateGenreAjaxAsync([FromBody] GenreDto dto) => new JsonResult(new { success = await _service.CreateGenreAsync(dto) });
        public async Task<IActionResult> OnPostUpdateGenreAjaxAsync([FromBody] GenreDto dto) => new JsonResult(new { success = await _service.UpdateGenreAsync(dto) });
        public async Task<IActionResult> OnPostDeleteGenreAjaxAsync(int id) => new JsonResult(new { success = await _service.DeleteGenreAsync(id) });

        public async Task<IActionResult> OnGetSeatCategoriesAjaxAsync() => new JsonResult(await _service.GetSeatCategoriesAsync());
        public async Task<IActionResult> OnPostCreateSeatCategoryAjaxAsync([FromBody] SeatCategoryDto dto) => new JsonResult(new { success = await _service.CreateSeatCategoryAsync(dto) });
        public async Task<IActionResult> OnPostUpdateSeatCategoryAjaxAsync([FromBody] SeatCategoryDto dto) => new JsonResult(new { success = await _service.UpdateSeatCategoryAsync(dto) });
        public async Task<IActionResult> OnPostDeleteSeatCategoryAjaxAsync(int id) => new JsonResult(new { success = await _service.DeleteSeatCategoryAsync(id) });

        public async Task<IActionResult> OnGetAmenitiesAjaxAsync() => new JsonResult(await _service.GetAmenitiesAsync());
        public async Task<IActionResult> OnPostCreateAmenityAjaxAsync([FromBody] AmenityDto dto) => new JsonResult(new { success = await _service.CreateAmenityAsync(dto) });
        public async Task<IActionResult> OnPostUpdateAmenityAjaxAsync([FromBody] AmenityDto dto) => new JsonResult(new { success = await _service.UpdateAmenityAsync(dto) });
        public async Task<IActionResult> OnPostDeleteAmenityAjaxAsync(int id) => new JsonResult(new { success = await _service.DeleteAmenityAsync(id) });

        public async Task<IActionResult> OnGetCertificatesAjaxAsync() => new JsonResult(await _service.GetCertificatesAsync());
        public async Task<IActionResult> OnPostCreateCertificateAjaxAsync([FromBody] CertificateDto dto) => new JsonResult(new { success = await _service.CreateCertificateAsync(dto) });
        public async Task<IActionResult> OnPostUpdateCertificateAjaxAsync([FromBody] CertificateDto dto) => new JsonResult(new { success = await _service.UpdateCertificateAsync(dto) });
        public async Task<IActionResult> OnPostDeleteCertificateAjaxAsync(int id) => new JsonResult(new { success = await _service.DeleteCertificateAsync(id) });
    }
}
