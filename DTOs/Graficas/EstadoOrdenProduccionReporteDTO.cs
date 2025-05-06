namespace caobaModeloFabricacion.DTOs.Graficas
{
    public class EstadoOrdenProduccionReporteDTO
    {
        public string? Estado { get; set; } // Ej: "En proceso", "Finalizada", "Pendiente"
        public int CantidadOrdenes { get; set; }
    }
}
