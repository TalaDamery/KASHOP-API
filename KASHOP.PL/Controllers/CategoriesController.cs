using KASHOP.BLL.Service;
using KASHOP.DAL.Data;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using KASHOP.PL.Resources;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoriesController : ControllerBase
    {
        private readonly IStringLocalizer<SharedResources> _localizer;
        //ال controller بيحكي مع ال service مش مع ال repository عشان ال service هو اللي بيحتوي على ال logic و ال repository هو اللي بيتعامل مع الداتا فقط
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService, IStringLocalizer<SharedResources> localizer)
        {
            _localizer = localizer;
            _categoryService = categoryService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryRequest request)
        {
            var response = await _categoryService.CreateCategory(request);
            return Ok(new { massege = _localizer["Success"].Value, response });
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var categories = await _categoryService.GetAllCategories();
            return Ok(new { data = categories, _localizer["Success"].Value });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _categoryService.GetCategory(c => c.Id == id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryService.DeleteCategory(id);
            if (!deleted)
                return NotFound(new { message = _localizer["NotFound"].Value });
            return Ok(new { message = _localizer["Success"].Value });

        }
    }
}
