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
        public DbSet<Operario> Operario { get; set; }
        public DbSet<OrdenMaterial> OrdenMaterial { get; set; }
        public DbSet<OrdenProduccion> OrdenProduccion { get; set; }
        public DbSet<Producto> Producto  { get; set; }
        public DbSet<Reporte> Reporte { get; set; }
        public DbSet<SeguimientoProduccion> SeguimientoProduccion { get; set; }
        public DbSet<OrdenMaterial> OrdenMateriales { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Elimina seguimientos al eliminar una orden
            modelBuilder.Entity<SeguimientoProduccion>()
                .HasOne(s => s.OrdenProduccion)
                .WithMany(o => o.Seguimientos)
                .HasForeignKey(s => s.OrdenProduccionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Elimina materiales asignados al eliminar una orden
            modelBuilder.Entity<OrdenMaterial>()
                .HasOne(om => om.Orden)
                .WithMany(o => o.Materiales)
                .HasForeignKey(om => om.Ordenid)
                .OnDelete(DeleteBehavior.Cascade);

            // Previene que se eliminen materiales reales si están vinculados a una orden
            modelBuilder.Entity<OrdenMaterial>()
                .HasOne(om => om.Material)
                .WithMany(m => m.OrdenMaterials)
                .HasForeignKey(om => om.Materialid)
                .OnDelete(DeleteBehavior.Restrict);

            // Elimina reportes al eliminar una orden
            modelBuilder.Entity<Reporte>()
            .HasOne(r => r.Orden)
            .WithMany(o => o.Reportes)
            .HasForeignKey(r => r.OrdenId)
            .OnDelete(DeleteBehavior.Cascade);


        }
        public DbSet<caobaModeloFabricacion.Models.Reporte> Reporte_1 { get; set; } = default!;

    }
}