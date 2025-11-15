namespace ControlAsistenciaAPI.Models
{
    public class ReporteRequest
    {
        public int? IdDocente { get; set; }
        public int? IdCarrera { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}