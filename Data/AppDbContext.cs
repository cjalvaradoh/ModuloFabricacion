using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using caobaModeloFabricacion.Models;
namespace caobaModeloFabricacion.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        {
        }
        public DbSet<CategoriaProducto> CategoriaProducto { get; set; }
        public DbSet<CategoriaProducto> Departamento { get; set; }
        public DbSet<DetalleProductoMaterial> DetalleProductoMaterial { get; set; }
        public DbSet<DetalleProductoMaterial> Maquina { get; set; }
        public DbSet<Material> Material { get; set; }
        public DbSet<Material> Operario { get; set; }
        public DbSet<Material> OrdenMaterial { get; set; }
        public DbSet<Material> OrdenProduccion { get; set; }
        public DbSet<Producto> Producto  { get; set; }
        public DbSet<Material> Reporte { get; set; }
        public DbSet<Material> SeguiminetoProduccion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

    }
}