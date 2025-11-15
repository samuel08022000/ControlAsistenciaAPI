namespace ControlAsistenciaAPI.Models
{
    public class ExportRequest
    {
        public int? IdDocente { get; set; }
        public int? IdCarrera { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string TipoReporte { get; set; } // "asistencia-docente", "asistencia-carrera", "estadisticas"
        public string Formato { get; set; } // "pdf", "excel"
    }
}