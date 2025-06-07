namespace caobaModeloFabricacion.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalOrdenes { get; set; }
        public decimal CostoTotalProduccion { get; set; }
        public int EficienciaProduccion { get; set; }

        public List<OperarioDTO> Operarios { get; set; } = new List<OperarioDTO>();
    }

    public class OperarioDTO
    {
        public string? Carnet { get; set; }
        public string? Nombre { get; set; }
        public bool? Activo { get; set; }
        public string? TiempoProduccion { get; set; }
        public string? Departamento { get; set; }
        public string? ImagenUrl { get; set; }
    }
}

