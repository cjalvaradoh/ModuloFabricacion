namespace caobaModeloFabricacion.DTOs.Reporte
{
    public class OrdenReporteDTO
    {
        public int OrdenId { get; set; }
        public int ProductoId { get; set; }
        public string? ProductoNombre { get; set; }
        public decimal Cantidad { get; set; }
        public string? Estado { get; set; }
    }
}
