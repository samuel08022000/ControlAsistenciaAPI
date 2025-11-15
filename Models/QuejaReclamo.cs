namespace ControlAsistenciaAPI.Models
{
    public class QuejaReclamo
    {
        public int IdQueja { get; set; }
        public int IdDocente { get; set; }
        public DateTime FechaQueja { get; set; }
        public string Tipo { get; set; } // "Queja", "Reclamo"
        public string Descripcion { get; set; }
        public string Estado { get; set; } // "Pendiente", "En Proceso", "Resuelto"
        public string Respuesta { get; set; }
    }

    public class QuejaReclamoRequest
    {
        public int IdDocente { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
    }
}