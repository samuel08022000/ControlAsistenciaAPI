namespace ControlAsistenciaAPI.Models
{
    public class Docente
    {
        public int IdDocente { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public int IdCarrera { get; set; }
        public string NombreCarrera { get; set; }
    }
}