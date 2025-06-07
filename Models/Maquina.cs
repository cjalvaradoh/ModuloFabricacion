using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace caobaModeloFabricacion.Models
{
    [Table("maquinas")]
    public class Maquina
    {
        [Key]
        [Column("id_maquina")]
        public int MaquinaId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Required]
        [Column("id_departamento")]
        public int DepartamentoId { get; set; }

        [ForeignKey("DepartamentoId")]
        public Departamento? Departamento { get; set; }
    }
}
