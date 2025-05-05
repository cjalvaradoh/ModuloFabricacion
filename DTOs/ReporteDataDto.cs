namespace caobaModeloFabricacion.DTOs
{
    public class ReporteDataDto
    {
        public int OrdenesEnProceso { get; set; }
        public int OrdenesFinalizadas { get; set; }
        public int OrdenesPendientes { get; set; }
        public int OrdenesCanceladas { get; set; }
        public double IndiceRetrasos { get; set; }
    }
}
