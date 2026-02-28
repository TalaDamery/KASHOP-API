using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class CategoryService : ICategoryService
    {
        // معناها لما تشوفني بعمل object من نوع ICategoryService روح انشئ object من نوع categoryRepository
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<CategoryResponse> CreateCategory(CategoryRequest request)
        {
            var category = request.Adapt<Category>();
            await _categoryRepository.CreateAsync(category);
            return category.Adapt<CategoryResponse>();
        }

        public async Task<List<CategoryResponse>> GetAllCategories()
        {
            // هون ال repository بيرجعلي list of category 
            var categories = await _categoryRepository.GetAllAsync();
            // هون حولت ال categories اللي جايه من ال repository الى list of category response عشان اقدر ارجعها لل api
            return categories.Adapt<List<CategoryResponse>>();
        }
    }
}
