using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace caobaModeloFabricacion.Models
{
    [Table("orden_materiales")]
    public class OrdenMaterial
    {
        [Key]
        [Column("id_orden_material")]
        public int? OrdenMaterialId { get; set; }

        [Required]
        [Column("id_orden")]
        public int Ordenid { get; set; }

        [Required]
        [Column("id_material")]
        public int Materialid { get; set; }

        [Required]
        [Column("cantidad_utilizada")]
        public decimal CantidadUtilizada { get; set; }

        [Required]
        [Column("desperdicio")]
        public decimal Desperdicio { get; set; }

        [ForeignKey("Ordenid")]
        public OrdenProduccion? Orden { get; set; }

        [ForeignKey("Materialid")]
        public Material? Material { get; set; }
    }
}
