using KASHOP.BLL.Mapping;
using KASHOP.BLL.Service;
using KASHOP.DAL.Data;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using KASHOP.DAL.utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            //ال services بتاخد ال context وبتتعامل معه بلأفضل طريقة ممكنة
            builder.Services.AddDbContext<ApplicationDbContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddLocalization(options => options.ResourcesPath = "");

            const string defaultCulture = "en";

            var supportedCultures = new[]
            {
                new CultureInfo(defaultCulture),
                new CultureInfo("ar")
            };

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(defaultCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Clear();

                //options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider
                //{
                //      QueryStringKey = "lang",
                //});

                options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
            });

            // هون عشان اخلي كل البروجيكتس يشتغلو على ال CategoryRepository
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            // لو بدي اخليهم يشتغلو على ال MockCategoryRep
            //builder.Services.AddScoped<ICategoryRepository, MockCategoryRep>();

            //لما تشوفني بتعامل مع ال ICategoryService اعرف انه بدك تعمل object من نوع CategoryService



            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IBrandRepository, BrandRepository>();
            builder.Services.AddScoped<IBrandService, BrandService>();

            builder.Services.AddScoped<IAuthenicationService, AuthenicationService>();
            builder.Services.AddScoped<ISeedData, RoleSeedData>();
            //هون الفرق انه هاد ال object مش رح يكمل معي للاخر
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            //هاي حطيتها عشان اعمل access ل appsettings من ال services 
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      //هون بضيف رابط الفرونت اند
                                      //policy.WithOrigins("hhttp://example.com",
                                      //                    "hhttp://www.contoso.com");
                                      policy.AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .AllowAnyOrigin();
                                  });
            });

            builder.Services.AddIdentity<ApplicationUser,IdentityRole>( Options =>
            {
                Options.User.RequireUniqueEmail = true;
                //identity options
                Options.Password.RequireDigit = true;//0-9
                Options.Password.RequireLowercase = true;//a-z
                Options.Password.RequireUppercase = true;//A-Z
                Options.Password.RequireNonAlphanumeric = true;//الرموز
                Options.Password.RequiredLength = 5;
                Options.Lockout.MaxFailedAccessAttempts = 5;
                //هون بيعمله بلوك لمدة 5 دقائق بعد ما يجرب 5 مرات يدخل الباسوورد
                Options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            }


                ).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                      .AddEnvironmentVariables();

            //وظيفته يفحص التوكن اذا كان صالح ولا لا قبل ما يسمح لل user يوصل لل api
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;//هون خلى الديفولت jwt بدل ال cookie عشان كل ال authentication يكون عن طريق ال jwt
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //هون اذا اليوزر ما عند توكن بيرجع 401 بدل 404
            })

                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = builder.Configuration["Jwt:Issuer"],
                            ValidAudience = builder.Configuration["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
                        };
                    });

            builder.Services.AddAuthorization();
            MapsterConfig.MapsterConfigRegister();
            builder.Services.AddHttpContextAccessor();



            var app = builder.Build();

            app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseAuthorization();
            app.UseCors(MyAllowSpecificOrigins);

            //هون بسمح لل api انه توصل للملفات الثابتة مثل الصور
            app.UseStaticFiles();

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var seeders = services.GetServices<ISeedData>();
                foreach (var seeder in seeders)
                {
                    //هاي الفنكشن الي احنا عملناها ب ISeedData
                   await seeder.DataSeed();
                }
            }

            app.Run();
        }
    }
}
