using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace caobaModeloFabricacion.Models
{
    [Table("reportes")]
    public class Reporte
    {
        [Key]
        [Column("id_reporte")]
        public int ReporteId { get; set; }

        [Column("id_orden")]
        public int OrdenId { get; set; }

        [Column("id_producto")]
        public int ProductoId { get; set; }

        [Required]
        [Column("cantidad")]
        public decimal Cantidad { get; set; }

        [Column("tiempo_produccion")]
        public decimal TiempoProduccion { get; set; }

        [Column("estado")]
        public string? Estado { get; set; } = string.Empty;

        [Column("fecha_generacion")]
        public DateTime? FechaGeneracion { get; set; }

        [ForeignKey("OrdenId")]
        public OrdenProduccion? Orden { get; set; }

        [ForeignKey("ProductoId")]
        public Producto? Producto { get; set; }
    }
}
