using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public interface IBrandService
    {
        Task CreateBrand(BrandRequest request);
        Task<List<BrandResponse>> GetAllBrandsAsync();
        Task<BrandResponse?> GetBrandAsync(Expression<Func<Brand, bool>> filter);
        Task<bool> DeleteBrandAsync(int id);
    }
}
