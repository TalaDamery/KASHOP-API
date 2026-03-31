using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.DTO.Response
{
    public class RegisterResponse
    {
        public string messsage { get; set; }
        public bool Success { get; set; }

        //هاي الليست بتحتوي على كل الايرورز اللي صارت في عملية التسجيل
        public List<string?> Errors { get; set; }


    }
}
