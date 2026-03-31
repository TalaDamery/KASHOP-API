using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public interface IFileService
    {
            Task<string> UploadFileAsync(IFormFile file);
            void Delete(string fileName);


    }
}
