namespace ControlAsistenciaAPI.Models
{
    public class Permiso
    {
        public int IdPermiso { get; set; }
        public int IdDocente { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaPermiso { get; set; }
        public string Motivo { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
        public string NombreDocente { get; set; }
    }
}