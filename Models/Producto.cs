using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace caobaModeloFabricacion.Models
{
    [Table("productos")]
    public class Producto
    {
        [Key]
        [Column("id_producto")]
        public int ProductoId { get; set; }

        [Required]
        [StringLength(10)]
        [Column("codigo")]
        public string? Codigo { get; set; }

        [Required]
        [StringLength(255)]
        [Column("nombre")]
        public string? Nombre { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Required]
        [StringLength(50)]
        [Column("unidad_medida")]
        public string? UnidadMedida { get; set; }

        [Column("tiempo_estimado_produccion")]
        public decimal TiempoEstimadoProduccion { get; set; }

        [Column("id_categoria")]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public CategoriaProducto? CategoriaProducto { get; set; }
    }
}