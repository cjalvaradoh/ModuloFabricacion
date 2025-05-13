namespace caobaModeloFabricacion.DTOs.Reporte
{
    public class OrdenReportePdfDto
    {
        public int OrdenId { get; set; }
        public string? CodigoProducto { get; set; }
        public string? NombreProducto { get; set; }
        public decimal Cantidad { get; set; }
        public string? Estado { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public decimal? TiempoProduccion { get; set; }
        public DateTime? FechaGeneracion { get; set; }

    }
}
