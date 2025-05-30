using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace caobaModeloFabricacion.Models
{
    [Table("detalle_producto_material")]
    public class DetalleProductoMaterial
    {
        [Key]
        [Column("id_detalle")]
        public int DetalleId { get; set; }

        [Required]
        [Column("id_producto")]
        public int ProductoId { get; set; }

        [Required]
        [Column("id_material")]
        public int MaterialId { get; set; }

        [Required]
        [Column("cantidad_requerida")]
        public decimal CantidadRequerida { get; set; }

        [ForeignKey("ProductoId")]
        public Producto? Producto { get; set; }

        [ForeignKey("MaterialId")]
        public Material? Material { get; set; }
    }
}
