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
        public static async Task Main(string[] args) // 👈 async para permitir await
        {
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            var builder = WebApplication.CreateBuilder(args);

            // Configuración de la cadena de conexión
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Configuración de Identity
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

            // Configuración de Cloudinary
            var cloudinaryAccount = new Account(
                Environment.GetEnvironmentVariable("CLOUDINARY_NAME"),
                Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"),
                Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
            );
            var cloudinary = new Cloudinary(cloudinaryAccount);
            builder.Services.AddSingleton(cloudinary);
            builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

            // MVC
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Middlewares
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

            // ✅ Crear roles automáticamente si no existen
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roles = new[] { "Admin", "User" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                        Console.WriteLine($"Rol creado: {role}");
                    }
                }
            }

            await app.RunAsync(); // 👈 Necesario por async Main
        }
    }
}
