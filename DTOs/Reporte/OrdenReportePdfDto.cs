namespace caobaModeloFabricacion.DTOs.Reporte
{
    public class OrdenReportePdfDto
    {
        // Datos básicos
        public int OrdenId { get; set; }
        public string? Estado { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public double DiasTranscurridos => (DateTime.Now - FechaInicio).TotalDays;

        // Producto (objeto anidado)
        public ProductoDto? Producto { get; set; }

        // Colecciones
        public List<SeguimientoDto> Seguimientos { get; set; } = new();
        public List<MaterialDto> Materiales { get; set; } = new();

        // Calculados
        public decimal CostoMateriales => Materiales.Sum(m => m.Costo);
        public decimal CostoManoObra => Seguimientos.Sum(s => s.TiempoTrabajado * s.TarifaOperario);
        public decimal CostoTotal => CostoMateriales + CostoManoObra;
        public decimal ProgresoPromedio => Seguimientos.Any() ? Seguimientos.Average(s => s.Avance) : 0m;

    }

    public class ProductoDto
    {
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
    }

    public class SeguimientoDto
    {
        public string? OperarioNombre { get; set; }
        public string? OperarioCarnet { get; set; }
        public decimal Avance { get; set; }
        public decimal TiempoTrabajado { get; set; }
        public decimal TarifaOperario { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string? Estado { get; set; }
    }

    public class MaterialDto
    {
        public string? Codigo { get; set; }
        public string? Nombre { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal Costo => Cantidad * CostoUnitario;
    }

}
