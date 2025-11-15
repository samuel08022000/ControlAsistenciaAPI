using Microsoft.AspNetCore.Mvc;
using ControlAsistenciaAPI.Models;
using ControlAsistenciaAPI.Services;

namespace ControlAsistenciaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IDatabaseService _databaseService;
        private readonly IExportService _exportService;

        public ReportesController(IDatabaseService databaseService, IExportService exportService)
        {
            _databaseService = databaseService;
            _exportService = exportService;
        }

        [HttpGet("docentes")]
        public async Task<ActionResult<IEnumerable<Docente>>> GetDocentes()
        {
            try
            {
                var docentes = await _databaseService.GetDocentesAsync();
                return Ok(docentes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener docentes: {ex.Message}");
            }
        }

        [HttpGet("carreras")]
        public async Task<ActionResult<IEnumerable<Carrera>>> GetCarreras()
        {
            try
            {
                var carreras = await _databaseService.GetCarrerasAsync();
                return Ok(carreras);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener carreras: {ex.Message}");
            }
        }

        [HttpPost("asistencia-docente")]
        public async Task<ActionResult<IEnumerable<Asistencia>>> GetAsistenciaPorDocente([FromBody] ReporteRequest request)
        {
            try
            {
                if (request.IdDocente == null)
                    return BadRequest("IdDocente es requerido");

                var asistencias = await _databaseService.GetAsistenciasPorDocenteAsync(
                    request.IdDocente.Value, request.FechaInicio, request.FechaFin);

                return Ok(asistencias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener asistencias: {ex.Message}");
            }
        }

        [HttpPost("asistencia-carrera")]
        public async Task<ActionResult<IEnumerable<Asistencia>>> GetAsistenciaPorCarrera([FromBody] ReporteRequest request)
        {
            try
            {
                if (request.IdCarrera == null)
                    return BadRequest("IdCarrera es requerido");

                var asistencias = await _databaseService.GetAsistenciasPorCarreraAsync(
                    request.IdCarrera.Value, request.FechaInicio, request.FechaFin);

                return Ok(asistencias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener asistencias por carrera: {ex.Message}");
            }
        }

        [HttpPost("estadisticas-asistencia")]
        public async Task<ActionResult<IEnumerable<Estadistica>>> GetEstadisticasAsistencia([FromBody] ReporteRequest request)
        {
            try
            {
                var estadisticas = await _databaseService.GetEstadisticasAsistenciaAsync(
                    request.FechaInicio, request.FechaFin);

                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener estadísticas: {ex.Message}");
            }
        }

        [HttpPost("estadisticas-permisos")]
        public async Task<ActionResult<IEnumerable<Estadistica>>> GetEstadisticasPermisos([FromBody] ReporteRequest request)
        {
            try
            {
                var estadisticas = await _databaseService.GetEstadisticasPermisosAsync(
                    request.FechaInicio, request.FechaFin);

                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener estadísticas de permisos: {ex.Message}");
            }
        }

        [HttpPost("exportar")]
        public async Task<IActionResult> ExportarReporte([FromBody] ExportRequest request)
        {
            try
            {
                byte[] fileBytes;
                string contentType;
                string fileName;

                switch (request.TipoReporte.ToLower())
                {
                    case "asistencia-docente":
                        if (request.IdDocente == null)
                            return BadRequest("IdDocente es requerido");

                        var asistenciasDocente = await _databaseService.GetAsistenciasPorDocenteAsync(
                            request.IdDocente.Value, request.FechaInicio, request.FechaFin);

                        if (request.Formato.ToLower() == "excel")
                        {
                            fileBytes = _exportService.GenerateExcelReport(asistenciasDocente,
                                $"Reporte de Asistencia - Docente {request.IdDocente}");
                            contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            fileName = $"Asistencia_Docente_{request.IdDocente}_{DateTime.Now:yyyyMMdd}.xlsx";
                        }
                        else
                        {
                            fileBytes = _exportService.GeneratePdfReport(asistenciasDocente,
                                $"Reporte de Asistencia - Docente {request.IdDocente}");
                            contentType = "application/pdf";
                            fileName = $"Asistencia_Docente_{request.IdDocente}_{DateTime.Now:yyyyMMdd}.pdf";
                        }
                        break;

                    case "asistencia-carrera":
                        if (request.IdCarrera == null)
                            return BadRequest("IdCarrera es requerido");

                        var asistenciasCarrera = await _databaseService.GetAsistenciasPorCarreraAsync(
                            request.IdCarrera.Value, request.FechaInicio, request.FechaFin);

                        if (request.Formato.ToLower() == "excel")
                        {
                            fileBytes = _exportService.GenerateExcelReport(asistenciasCarrera,
                                $"Reporte de Asistencia - Carrera {request.IdCarrera}");
                            contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            fileName = $"Asistencia_Carrera_{request.IdCarrera}_{DateTime.Now:yyyyMMdd}.xlsx";
                        }
                        else
                        {
                            fileBytes = _exportService.GeneratePdfReport(asistenciasCarrera,
                                $"Reporte de Asistencia - Carrera {request.IdCarrera}");
                            contentType = "application/pdf";
                            fileName = $"Asistencia_Carrera_{request.IdCarrera}_{DateTime.Now:yyyyMMdd}.pdf";
                        }
                        break;

                    case "estadisticas-asistencia":
                        var statsAsistencia = await _databaseService.GetEstadisticasAsistenciaAsync(
                            request.FechaInicio, request.FechaFin);

                        if (request.Formato.ToLower() == "excel")
                        {
                            fileBytes = _exportService.GenerateExcelStats(statsAsistencia,
                                "Estadísticas de Asistencia");
                            contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            fileName = $"Estadisticas_Asistencia_{DateTime.Now:yyyyMMdd}.xlsx";
                        }
                        else
                        {
                            fileBytes = _exportService.GeneratePdfStats(statsAsistencia,
                                "Estadísticas de Asistencia");
                            contentType = "application/pdf";
                            fileName = $"Estadisticas_Asistencia_{DateTime.Now:yyyyMMdd}.pdf";
                        }
                        break;

                    case "estadisticas-permisos":
                        var statsPermisos = await _databaseService.GetEstadisticasPermisosAsync(
                            request.FechaInicio, request.FechaFin);

                        if (request.Formato.ToLower() == "excel")
                        {
                            fileBytes = _exportService.GenerateExcelStats(statsPermisos,
                                "Estadísticas de Permisos");
                            contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            fileName = $"Estadisticas_Permisos_{DateTime.Now:yyyyMMdd}.xlsx";
                        }
                        else
                        {
                            fileBytes = _exportService.GeneratePdfStats(statsPermisos,
                                "Estadísticas de Permisos");
                            contentType = "application/pdf";
                            fileName = $"Estadisticas_Permisos_{DateTime.Now:yyyyMMdd}.pdf";
                        }
                        break;

                    default:
                        return BadRequest("Tipo de reporte no válido");
                }

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al generar reporte: {ex.Message}");
            }
        }

        [HttpPost("quejas-reclamos")]
        public async Task<ActionResult> CrearQuejaReclamo([FromBody] QuejaReclamoRequest request)
        {
            try
            {
                var queja = new QuejaReclamo
                {
                    IdDocente = request.IdDocente,
                    FechaQueja = DateTime.Now,
                    Tipo = request.Tipo,
                    Descripcion = request.Descripcion,
                    Estado = "Pendiente"
                };

                var id = await _databaseService.CreateQuejaReclamoAsync(queja);
                return Ok(new { IdQueja = id, Mensaje = "Queja/reclamo registrado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar queja/reclamo: {ex.Message}");
            }
        }

        [HttpGet("permisos")]
        public async Task<ActionResult<IEnumerable<Permiso>>> GetPermisos([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var permisos = await _databaseService.GetPermisosAsync(fechaInicio, fechaFin);
                return Ok(permisos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener permisos: {ex.Message}");
            }
        }
    }
}