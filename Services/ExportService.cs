using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ControlAsistenciaAPI.Models;

namespace ControlAsistenciaAPI.Services
{
    public interface IExportService
    {
        byte[] GenerateExcelReport(IEnumerable<Asistencia> asistencias, string titulo);
        byte[] GeneratePdfReport(IEnumerable<Asistencia> asistencias, string titulo);
        byte[] GenerateExcelStats(IEnumerable<Estadistica> estadisticas, string titulo);
        byte[] GeneratePdfStats(IEnumerable<Estadistica> estadisticas, string titulo);
    }

    public class ExportService : IExportService
    {
        public byte[] GenerateExcelReport(IEnumerable<Asistencia> asistencias, string titulo)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reporte de Asistencia");

            // Título
            worksheet.Cell(1, 1).Value = titulo;
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 7).Merge();

            // Encabezados
            var headers = new[] { "ID", "Docente", "Fecha", "Hora Llegada", "Estado", "Observaciones", "Horario ID" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(3, i + 1).Value = headers[i];
                worksheet.Cell(3, i + 1).Style.Font.Bold = true;
                worksheet.Cell(3, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            // Datos
            int row = 4;
            foreach (var asistencia in asistencias)
            {
                worksheet.Cell(row, 1).Value = asistencia.IdAsistencia;
                worksheet.Cell(row, 2).Value = asistencia.NombreDocente;
                worksheet.Cell(row, 3).Value = asistencia.Fecha.ToString("dd/MM/yyyy");
                worksheet.Cell(row, 4).Value = asistencia.HoraLlegada.ToString(@"hh\:mm");
                worksheet.Cell(row, 5).Value = asistencia.EstadoAsistencia;
                worksheet.Cell(row, 6).Value = asistencia.Observaciones;
                worksheet.Cell(row, 7).Value = asistencia.IdHorario;
                row++;
            }

            // Autoajustar columnas
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] GeneratePdfReport(IEnumerable<Asistencia> asistencias, string titulo)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Header().Text(titulo).SemiBold().FontSize(16).AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(2, Unit.Centimetre);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("ID").Bold();
                            header.Cell().Text("Docente").Bold();
                            header.Cell().Text("Fecha").Bold();
                            header.Cell().Text("Hora").Bold();
                            header.Cell().Text("Estado").Bold();
                            header.Cell().Text("Observaciones").Bold();
                        });

                        foreach (var asistencia in asistencias)
                        {
                            table.Cell().Text(asistencia.IdAsistencia.ToString());
                            table.Cell().Text(asistencia.NombreDocente);
                            table.Cell().Text(asistencia.Fecha.ToString("dd/MM/yyyy"));
                            table.Cell().Text(asistencia.HoraLlegada.ToString(@"hh\:mm"));
                            table.Cell().Text(asistencia.EstadoAsistencia);
                            table.Cell().Text(asistencia.Observaciones ?? "");
                        }
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Página ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateExcelStats(IEnumerable<Estadistica> estadisticas, string titulo)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Estadísticas");

            worksheet.Cell(1, 1).Value = titulo;
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 3).Merge();

            worksheet.Cell(3, 1).Value = "Categoría";
            worksheet.Cell(3, 2).Value = "Cantidad";
            worksheet.Cell(3, 3).Value = "Porcentaje (%)";

            for (int i = 1; i <= 3; i++)
            {
                worksheet.Cell(3, i).Style.Font.Bold = true;
                worksheet.Cell(3, i).Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            int row = 4;
            foreach (var stat in estadisticas)
            {
                worksheet.Cell(row, 1).Value = stat.Categoria;
                worksheet.Cell(row, 2).Value = stat.Cantidad;
                worksheet.Cell(row, 3).Value = stat.Porcentaje;
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] GeneratePdfStats(IEnumerable<Estadistica> estadisticas, string titulo)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Header().Text(titulo).SemiBold().FontSize(16).AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Categoría").Bold();
                            header.Cell().Text("Cantidad").Bold();
                            header.Cell().Text("Porcentaje (%)").Bold();
                        });

                        foreach (var stat in estadisticas)
                        {
                            table.Cell().Text(stat.Categoria);
                            table.Cell().Text(stat.Cantidad.ToString()).AlignRight();
                            table.Cell().Text($"{stat.Porcentaje:F2}%").AlignRight();
                        }
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Página ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}