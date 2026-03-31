using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IBrandService _brandService;
        public BrandsController(IBrandService brandService, IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
            _brandService = brandService;
        }


        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var Brands = await _brandService.GetAllBrandsAsync();
            return Ok(new { data = Brands });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _brandService.GetBrandAsync(p => p.Id == id);
            if (brand == null) return NotFound();
            return Ok(new { data = brand });
        }



        [HttpPost("")]
        [Authorize]
        //هون لما ارسل الداتا رح تنرسل عن طريق الفورم مش جيسون
        public async Task<IActionResult> Create([FromForm] BrandRequest request)
        {
            await _brandService.CreateBrand(request);
            return Ok();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _brandService.DeleteBrandAsync(id);
            if (!deleted)
                return NotFound(new { message = _localizer["NotFound"].Value });
            return Ok(new { message = _localizer["Success"].Value });

        }
    }
}
