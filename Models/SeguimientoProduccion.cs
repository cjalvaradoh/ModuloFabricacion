using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace caobaModeloFabricacion.Models
{
    [Table("seguimiento_produccion")]
    public class SeguimientoProduccion
    {
        [Key]
        [Column("id_seguimiento")]
        public int Id { get; set; }

        [Column("id_orden")]
        public int OrdenProduccionId { get; set; }

        [Column("id_operario")]
        public int OperarioId { get; set; }

        [Column("estado")]
        public string Estado { get; set; } = string.Empty;

        [Column("avance")]
        public decimal? Avance { get; set; }

        [Column("tiempo_trabajado")]
        public decimal? TiempoTrabajado { get; set; }

        [Column("material_consumido")]
        public decimal? MaterialConsumido { get; set; }

        [Column("fecha_actualizacion")]
        public DateTime? FechaActualizacion { get; set; }  // <-- ❗ este puede venir null

        [Column("comentarios")]
        public string? Comentarios { get; set; }  // <-- ❗ este puede venir null

        public OrdenProduccion OrdenProduccion { get; set; } = null!;
        public Operario Operario { get; set; } = null!;
    }
}

