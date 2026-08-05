namespace AuraDental.Aplicacion.Dtos
{
    public class EstadisticasDashboardDto
    {
        public List<string> EtiquetasSemanas { get; set; } = new();
        public List<int> CitasPorSemana { get; set; } = new();

        public List<string> NombresServicios { get; set; } = new();
        public List<int> CantidadPorServicio { get; set; } = new();

        public decimal IngresosEstimados { get; set; }
        public int TotalCitasCompletadas { get; set; }
    }
}