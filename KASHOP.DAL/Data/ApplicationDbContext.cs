using KASHOP.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Security.Claims;

namespace KASHOP.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryTranslation> CategoryTranslations { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductTranslation> ProductTranslations { get; set; }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<BrandTranslation> BrandTranslations { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor HttpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = HttpContextAccessor;
        }

        //طريقة بدل ال data annotations لتغيير اسماء الجداول في قاعدة البيانات
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<Category>().ToTable("Cats");

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UsersRoles");


            //هون مثلا اذا اجينا نحذف كاتيجوري في منتجات الها ما رح تنحذف لان ال delete behavior عندها restrict يعني رح يرفض الحذف اذا في منتجات مرتبطة بها
            //هون عشان كان في مشكلة لما عملت update-database وحكالي انه الcategory عندها علاقة مع الuser بس ما عرفها فهنا عرفت العلاقة بيناتهم
            builder.Entity<Category>()
                .HasOne(p => p.CreatedBy)
                .WithMany()
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Category>()
              .HasOne(p => p.UpdatedBy)
              .WithMany()
              .HasForeignKey(p => p.UpdatedById)
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
               .HasOne(p => p.CreatedBy)
               .WithMany()
               .HasForeignKey(p => p.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
              .HasOne(p => p.UpdatedBy)
              .WithMany()
              .HasForeignKey(p => p.UpdatedById)
              .OnDelete(DeleteBehavior.Restrict);
            /*
                هنا، إذا حاولت تحذف Brand وهو عنده منتجات مرتبطة، EF Core رح يمنعك.
                لو حبيت تحذف Brand مع كل منتجاته، تستخدم Cascade بدل Restrict
             */
            builder.Entity<Brand>()
              .HasOne(p => p.CreatedBy)
              .WithMany()
              .HasForeignKey(p => p.CreatedById)
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Brand>()
              .HasOne(p => p.UpdatedBy)
              .WithMany()
              .HasForeignKey(p => p.UpdatedById)
              .OnDelete(DeleteBehavior.Restrict);

        }

        //اي فنكشن بتعمل SaveChanges تعال هون واعمل هاي الشغلات
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_httpContextAccessor.HttpContext != null)
            {

                var entries = ChangeTracker.Entries<AuditableEntity>();

                var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);


                foreach (var entry in entries)
                {
                    // اذا كان السجل جديد
                    if (entry.State == EntityState.Added)
                    {
                        // حط وقت الانشاء تلقائي
                        entry.Property(x => x.CreatedById).CurrentValue = currentUserId;
                        entry.Property(x => x.CreatedOn).CurrentValue = DateTime.UtcNow;

                    }
                    if (entry.State == EntityState.Modified)
                    {
                        // حط وقت التعديل تلقائي
                        entry.Property(x => x.UpdatedById).CurrentValue = currentUserId;
                        entry.Property(x => x.UpdatedOn).CurrentValue = DateTime.UtcNow;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}