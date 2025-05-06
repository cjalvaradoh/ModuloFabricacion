namespace caobaModeloFabricacion.DTOs.Graficas
{
    public class AvanceProduccionReporteDTO
    {
        public int IdOrden { get; set; }
        public string? Producto { get; set; }
        public string? Categoria { get; set; }
        public string? EstadoOrden { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public decimal? TiempoProduccionReportado { get; set; } // Desde tabla reportes
        public bool EsRetrasada =>
            FechaEntrega.HasValue && FechaEntrega.Value < DateTime.Now && EstadoOrden != "Finalizada";
    }
}
