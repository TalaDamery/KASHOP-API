using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Mapping
{
    //هون الكلاس بدي اخليه static عشان اقدر استدعيه من اي مكان في ال project بدون ما اعمل object منه
    //بستدعيه بال program.cs عشان يشتغل اول ما يشتغل ال project
    public static class MapsterConfig
    {
        public static void MapsterConfigRegister()
        {
            //لما تشوفني بحول من category الى category response 
            TypeAdapterConfig<Category, CategoryResponse>.NewConfig()
                .Map(dest => dest.Category_Id, src => src.Id)
                //هون الcreatedby فيها كل معلومات ال user ف اخدت بس اسمه 
                .Map(dest => dest.UserCreated, src => src.CreatedBy.UserName)
                .Map(dest => dest.Name, src => src.Translations.Where
                    (
                        //هون بدي اختار الترجمه اللي بتطابق ال culture الحالي عشان اعرض اسم الفئة باللغه اللي المستخدم مختارها
                        t => t.Language == CultureInfo.CurrentCulture.Name
                    ).Select(t => t.Name).FirstOrDefault()
                );

            TypeAdapterConfig<Product, ProductResponse>.NewConfig()
              .Map(dest => dest.UserCreated, src => src.CreatedBy.UserName)
              .Map(dest => dest.Name, src => src.Translations.Where
                 (
                     t => t.Language == CultureInfo.CurrentCulture.Name
                 ).Select(t => t.Name).FirstOrDefault()

              )
              .Map(dest => dest.MainImage, src => $"https://localhost:7132/images/{src.MainImage}");


            TypeAdapterConfig<Brand, BrandResponse>.NewConfig()
               .Map(dest => dest.UserCreated, src => src.CreatedBy.UserName)

               .Map(dest => dest.Name, src => src.Translations
                   .Where(t => t.Language == CultureInfo.CurrentCulture.Name)
                   .Select(t => t.Name)
                   .FirstOrDefault()
               )

               .Map(dest => dest.Logo, src => $"https://localhost:7132/images/{src.Logo}");

           
        }

    }
}
