using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace caobaModeloFabricacion.Models
{
    [Table("operarios")]
    public class Operario
    {
        [Key]
        [Column("id_operario")]
        public int Id { get; set; }

        [Column("carnet")]
        public string Carnet { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("estado")]
        public string Estado { get; set; }

        [Column("tiempo_hora")]
        public decimal? TiempoHora { get; set; }

        [Column("thumbnail_url")]
        public string ThumbnailUrl { get; set; }

        [Column("foto_url")]
        public string FotoUrl { get; set; }

        [Column("id_departamento")]
        public int? DepartamentoId { get; set; }

        // Relaciones 
        // public Departamento Departamento { get; set; }
    }
}