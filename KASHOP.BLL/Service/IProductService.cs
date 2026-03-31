using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public interface IProductService
    {
        Task CreateProduct(ProductRequest request);
        Task<List<ProductResponse>> GetAllProductsAsync();
        Task<ProductResponse?> GetProductAsync(Expression<Func<Product, bool>> filter);
        Task<bool> DeleteProductAsync(int id);
    }
}
