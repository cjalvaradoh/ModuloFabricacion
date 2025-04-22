using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace caobaModeloFabricacion.Models
{
    [Table("departamentos")]
    public class Departamento
    {

        [Key]
        [Column("id_departamento")]
        public int DepartamentoId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("nombre")]
        public string Nombre { get; set; }


    }
}
