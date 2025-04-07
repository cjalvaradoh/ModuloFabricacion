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

        public DbSet<Material> Material { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<CategoriaProducto> CategoriaProducto { get; set; }
        public DbSet<DetalleProductoMaterial> DetalleProductoMaterial { get; set; }

        public DbSet<OrdenProduccion> OrdenesProduccion { get; set; }
        public DbSet<SeguimientoProduccion> SeguimientosProduccion { get; set; }
        public DbSet<Operario> Operarios { get; set; } // 👈 NUEVO: Agregamos operarios

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 👇 Esta relación ya estaba bien:
            modelBuilder.Entity<SeguimientoProduccion>()
                .HasOne(s => s.OrdenProduccion)
                .WithMany(o => o.Seguimientos)
                .HasForeignKey(s => s.OrdenProduccionId);

            // 👇 Nueva relación opcional: seguimiento tiene un operario
            modelBuilder.Entity<SeguimientoProduccion>()
                .HasOne(s => s.Operario)
                .WithMany()
                .HasForeignKey(s => s.OperarioId);
        }
    }
}
