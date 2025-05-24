using caobaModeloFabricacion.Data;
using caobaModeloFabricacion.Models;
using caobaModeloFabricacion.Services; 
using CloudinaryDotNet;
using dotenv.net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace caobaModeloFabricacion
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            var builder = WebApplication.CreateBuilder(args);

            // Configuración de la cadena de conexión
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Identity (sin cambios)
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // ?? Configuración de Cloudinary (original)
             var cloudinaryAccount = new Account(
                 Environment.GetEnvironmentVariable("CLOUDINARY_NAME"),
                 Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"),
                Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
             );
             builder.Services.AddSingleton(new Cloudinary(cloudinaryAccount));


            var cloudinary = new Cloudinary(cloudinaryAccount);

            // ?? Registra los servicios (NUEVO)
            builder.Services.AddSingleton(cloudinary); // Registra Cloudinary directamente
            builder.Services.AddScoped<ICloudinaryService, CloudinaryService>(); // Registra tu wrapper

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configuración del pipeline (sin cambios)
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}/{id?}");

            app.Run();
        }
    }
}