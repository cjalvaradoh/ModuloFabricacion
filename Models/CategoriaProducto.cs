using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace caobaModeloFabricacion.Models
{
    [Table("categoria_productos")]
    public class CategoriaProducto
    {
        [Key]
        [Column("id_categoria")]
        public int CategoriaId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nombre")]
        public string? Nombre { get; set; }

        public ICollection<Producto>? Productos { get; set; }
    }
}
