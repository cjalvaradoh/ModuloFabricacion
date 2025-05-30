using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace caobaModeloFabricacion.Models
{
    [Table("ordenes_produccion")]
    public class OrdenProduccion
    {
        [Key]
        [Column("id_orden")]
        public int Ordenid { get; set; }

        [Column("id_producto")]
        public int Productoid { get; set; }

        [Required]
        [Column("cantidad")]
        public decimal Cantidad { get; set; }

        [Required]
        [StringLength(10)]
        [Column("estado")]
        public string Estado { get; set; } = "Pendiente";

        [Required]
        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }
        public ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();

        [Required]
        [Column("fecha_entrega")]
        public DateTime FechaEntrega { get; set; }

        [ForeignKey("Productoid")]
        public Producto? Producto { get; set; }

        public ICollection<SeguimientoProduccion> Seguimientos { get; set; } = new List<SeguimientoProduccion>();
        public ICollection<OrdenMaterial> Materiales { get; set; } = new List<OrdenMaterial>();
    }
}
