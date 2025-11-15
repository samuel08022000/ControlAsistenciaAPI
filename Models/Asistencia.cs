namespace ControlAsistenciaAPI.Models
{
    public class Asistencia
    {
        public int IdAsistencia { get; set; }
        public int IdDocente { get; set; }
        public int IdHorario { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraLlegada { get; set; }
        public string EstadoAsistencia { get; set; }
        public string Observaciones { get; set; }
        public string NombreDocente { get; set; }
    }
}