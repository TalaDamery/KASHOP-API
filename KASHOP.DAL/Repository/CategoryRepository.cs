using KASHOP.DAL.Data;
using KASHOP.DAL.Models;


namespace KASHOP.DAL.Repository
{
    // طبقت عليها برضو ال ICategoryRepository عشان اذا حتجت اشي اضافي خاص بالcategory
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) 
        {
        }
        
    }
}
