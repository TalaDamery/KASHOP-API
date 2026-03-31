using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class FileService : IFileService
    {
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","images", fileName);

                using (var stream = File.Create(filePath))
                {
                   await file.CopyToAsync(stream);
                }
                return fileName;
            }
            return null;
        }

        //هاي عملناها عشان لما نحذف بروداكت تنحذف الصورة الي اله 
        public void Delete (string fileName)
        {
                                        //هون بحكيله وقف على ال pl
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }


}
