using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Models
{
    //عملته عشان اضيف خصائص اضافية للمستخدم غير الخصائص الاساسية الموجودة في IdentityUser
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
    }
}
