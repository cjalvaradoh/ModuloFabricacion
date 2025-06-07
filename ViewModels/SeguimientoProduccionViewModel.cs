using System.ComponentModel.DataAnnotations;

namespace caobaModeloFabricacion.ViewModels
{
    public class SeguimientoProduccionViewModel
    {
        [Required(ErrorMessage = "La orden es obligatoria")]
        public int? OrdenProduccionId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un operario")]
        public int? OperarioId { get; set; }

        [Required(ErrorMessage = "El avance es obligatorio")]
        public decimal? Avance { get; set; }

        [Required(ErrorMessage = "El tiempo trabajado es obligatorio")]
        public decimal? TiempoTrabajado { get; set; }

        [Required(ErrorMessage = "El material consumido es obligatorio")]
        public decimal? MaterialConsumido { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un estado")]
        public string Estado { get; set; } = string.Empty;

        public string? Comentarios { get; set; }

        public int DepartamentoId { get; set; }
    }
}
