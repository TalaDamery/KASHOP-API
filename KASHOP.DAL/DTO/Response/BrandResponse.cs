using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.DTO.Response
{
    public class BrandResponse
    {
        public int Id { get; set; }

        public string UserCreated { get; set; } 
        public string Logo { get; set; }

        public string Name { get; set; }



    }
}
