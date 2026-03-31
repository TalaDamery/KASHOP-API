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
    public class ProductsController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        private readonly IProductService _productService;
        public ProductsController(IProductService productService, IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
            _productService = productService;
        }


        [HttpGet("")]
        public async Task<IActionResult> Index() {
            var products= await _productService.GetAllProductsAsync();
            return Ok(new {data=products});
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductAsync(p=> p.Id==id);
            if(product==null) return NotFound();
            return Ok(new { data = product });
        }


        [HttpPost("")]
        [Authorize]

        //هون لما ارسل الداتا رح تنرسل عن طريق الفورم مش جيسون
        public async Task<IActionResult> Create([FromForm] ProductRequest request)
        {
            await _productService.CreateProduct(request);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
                return NotFound(new { message = _localizer["NotFound"].Value });
            return Ok(new { message = _localizer["Success"].Value });

        }

    }
}
