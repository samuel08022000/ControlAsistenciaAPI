using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using ControlAsistenciaAPI.Models;

namespace ControlAsistenciaAPI.Services
{
    public interface IDatabaseService
    {
        // MÉTODOS EXISTENTES
        Task<IEnumerable<Docente>> GetDocentesAsync();
        Task<IEnumerable<Asistencia>> GetAsistenciasPorDocenteAsync(int idDocente, DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<Asistencia>> GetAsistenciasPorCarreraAsync(int idCarrera, DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<Estadistica>> GetEstadisticasAsistenciaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<Estadistica>> GetEstadisticasPermisosAsync(DateTime fechaInicio, DateTime fechaFin);

        // NUEVOS MÉTODOS
        Task<IEnumerable<Carrera>> GetCarrerasAsync();
        Task<IEnumerable<Permiso>> GetPermisosAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<int> CreateQuejaReclamoAsync(QuejaReclamo queja);
    }

    public class DatabaseService : IDatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // MÉTODOS EXISTENTES
        public async Task<IEnumerable<Docente>> GetDocentesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                SELECT d.IdDocente, d.Nombre, d.Apellido, d.Correo, d.Telefono, 
                       d.IdCarrera, c.NombreCarrera
                FROM Docentes d
                INNER JOIN Carreras c ON d.IdCarrera = c.IdCarrera";
            return await connection.QueryAsync<Docente>(sql);
        }

        public async Task<IEnumerable<Asistencia>> GetAsistenciasPorDocenteAsync(int idDocente, DateTime fechaInicio, DateTime fechaFin)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                SELECT a.IdAsistencia, a.IdDocente, a.IdHorario, a.Fecha, a.HoraLlegada, 
                       a.EstadoAsistencia, a.Observaciones,
                       CONCAT(d.Nombre, ' ', d.Apellido) as NombreDocente
                FROM Asistencias a
                INNER JOIN Docentes d ON a.IdDocente = d.IdDocente
                WHERE a.IdDocente = @IdDocente 
                AND a.Fecha BETWEEN @FechaInicio AND @FechaFin
                ORDER BY a.Fecha DESC";

            return await connection.QueryAsync<Asistencia>(sql, new
            {
                IdDocente = idDocente,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });
        }

        public async Task<IEnumerable<Asistencia>> GetAsistenciasPorCarreraAsync(int idCarrera, DateTime fechaInicio, DateTime fechaFin)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                SELECT a.IdAsistencia, a.IdDocente, a.IdHorario, a.Fecha, a.HoraLlegada, 
                       a.EstadoAsistencia, a.Observaciones,
                       CONCAT(d.Nombre, ' ', d.Apellido) as NombreDocente
                FROM Asistencias a
                INNER JOIN Docentes d ON a.IdDocente = d.IdDocente
                WHERE d.IdCarrera = @IdCarrera 
                AND a.Fecha BETWEEN @FechaInicio AND @FechaFin
                ORDER BY a.Fecha DESC";

            return await connection.QueryAsync<Asistencia>(sql, new
            {
                IdCarrera = idCarrera,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });
        }

        public async Task<IEnumerable<Estadistica>> GetEstadisticasAsistenciaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                SELECT 
                    EstadoAsistencia as Categoria,
                    COUNT(*) as Cantidad,
                    CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Asistencias WHERE Fecha BETWEEN @FechaInicio AND @FechaFin) as DECIMAL(5,2)) as Porcentaje
                FROM Asistencias 
                WHERE Fecha BETWEEN @FechaInicio AND @FechaFin
                GROUP BY EstadoAsistencia";

            return await connection.QueryAsync<Estadistica>(sql, new
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });
        }

        public async Task<IEnumerable<Estadistica>> GetEstadisticasPermisosAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                SELECT 
                    Estado as Categoria,
                    COUNT(*) as Cantidad,
                    CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Permisos WHERE FechaSolicitud BETWEEN @FechaInicio AND @FechaFin) as DECIMAL(5,2)) as Porcentaje
                FROM Permisos 
                WHERE FechaSolicitud BETWEEN @FechaInicio AND @FechaFin
                GROUP BY Estado";

            return await connection.QueryAsync<Estadistica>(sql, new
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });
        }

        // NUEVOS MÉTODOS
        public async Task<IEnumerable<Carrera>> GetCarrerasAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "SELECT IdCarrera, NombreCarrera, Facultad FROM Carreras";
            return await connection.QueryAsync<Carrera>(sql);
        }

        public async Task<IEnumerable<Permiso>> GetPermisosAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                SELECT p.IdPermiso, p.IdDocente, p.FechaSolicitud, p.FechaPermiso, 
                       p.Motivo, p.Estado, p.Observaciones,
                       CONCAT(d.Nombre, ' ', d.Apellido) as NombreDocente
                FROM Permisos p
                INNER JOIN Docentes d ON p.IdDocente = d.IdDocente
                WHERE p.FechaSolicitud BETWEEN @FechaInicio AND @FechaFin
                ORDER BY p.FechaSolicitud DESC";

            return await connection.QueryAsync<Permiso>(sql, new
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });
        }

        public async Task<int> CreateQuejaReclamoAsync(QuejaReclamo queja)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"
                INSERT INTO QuejasReclamos (IdDocente, FechaQueja, Tipo, Descripcion, Estado)
                VALUES (@IdDocente, @FechaQueja, @Tipo, @Descripcion, @Estado);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await connection.ExecuteScalarAsync<int>(sql, queja);
        }
    }
}