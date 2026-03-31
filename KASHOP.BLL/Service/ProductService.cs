using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileService _fileService;

        public ProductService(IProductRepository productRepository, IFileService fileService)
        {
            _productRepository = productRepository;
            _fileService = fileService;
        }
        public async Task CreateProduct(ProductRequest request)
        {
            var product = request.Adapt<Product>();
            if (request.MainImage != null)
            {
                var imagePath=await _fileService.UploadFileAsync(request.MainImage);
                product.MainImage = imagePath;
            }
            //هون الكرييت بتوخد دومين موديل عشان هيك عملت مابينج
            await _productRepository.CreateAsync(product);
        }

        public async Task<List<ProductResponse>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync(
               new string[]
               {
                  nameof(Product.Translations),
                  nameof(Product.CreatedBy)
               }
            );
          
            //هون لازم ارجع dtoعشان هيك عملت mapping
            return products.Adapt<List<ProductResponse>>();
        }

        public async Task<ProductResponse?> GetProductAsync(Expression<Func<Product, bool>> filter)
        {
            var product = await _productRepository.GetOne(filter,
                  new string[]
               {
                  nameof(Product.Translations),
                  nameof(Product.CreatedBy)
               });
            if (product == null) return null;
            return product.Adapt<ProductResponse>();
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetOne(c => c.Id == id);
            if (product == null) return false;
            _fileService.Delete(product.MainImage);
            return await _productRepository.DeleteAsync(product);
        }

    }
}
