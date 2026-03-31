using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.utils
{
    public class RoleSeedData : ISeedData
    {
        // لما تشوفني بعمل roleSeedData اعمل roleManager
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleSeedData(RoleManager<IdentityRole> roleManager) {
            _roleManager = roleManager;
        }
        public async Task DataSeed()
        {
            String [] roles = ["User", "Admin", "SuperAdmin" ];
            //افحص هل في اي داتا بالrole
            if(! await _roleManager.Roles.AnyAsync())
            {
                foreach (var role in roles)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
