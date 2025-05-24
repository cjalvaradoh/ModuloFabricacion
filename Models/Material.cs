using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace caobaModeloFabricacion.Models
{
    [Table("materiales")]
    public class Material
    {
        [Key]
        [Column("id_material")]
        public int MaterialId { get; set; }

        [Required]
        [StringLength(10)]
        [Column("codigo_material")]
        public string Codigo { get; set; }

        [Required]
        [StringLength(255)]
        [Column("nombre_material")]
        public string Nombre { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("stock")]
        public decimal Stock { get; set; }

        [Column("costo_por_unidad")]
        public decimal PrecioUnidad { get; set; }

        [Column("ancho")]
        public decimal? Ancho { get; set; }

        [Column("largo")]
        public decimal? Largo { get; set; }

        [Column("alto")]
        public decimal? Alto { get; set; }

        [Required(ErrorMessage = "El tipo es obligatorio")]
        [StringLength(20)]
        [Column("tipo")]
        public string Tipo { get; set; }

        [Column("thumbnail_url")]
        public string? ThumbnailUrl { get; set; }

        [Column("foto_url")]
        public string? FotoUrl { get; set; }

        public virtual ICollection<OrdenMaterial> OrdenMaterials { get; set; } = new List<OrdenMaterial>();
    }
}
