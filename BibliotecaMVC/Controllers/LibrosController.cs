using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class LibrosController : Controller
    {
        private readonly BibliotecaContext _context;
        private readonly ILogger<LibrosController> _logger;

        public LibrosController(BibliotecaContext context, ILogger<LibrosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Iniciando consulta de libros");
            try
            {
                var libros = await _context.Libros.ToListAsync();
                _logger.LogInformation("Consulta de libros completada");
                return View(libros);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al consultar los libros");
                throw;
            }
        }

        [Authorize(Roles = "Admin,Bibliotecario")]
        public async Task<IActionResult> Reporte()
        {
            var libros = await _context.Libros
                .AsNoTracking()
                .OrderBy(libro => libro.Titulo)
                .ToListAsync();

            return File(GenerarReporte(libros), "application/pdf");
        }

        [Authorize(Roles = "Admin,Bibliotecario")]
        public async Task<IActionResult> DescargarReporte()
        {
            var libros = await _context.Libros
                .AsNoTracking()
                .OrderBy(libro => libro.Titulo)
                .ToListAsync();

            return File(GenerarReporte(libros), "application/pdf", "Reporte_Libros.pdf");
        }

        [Authorize(Roles = "Admin,Bibliotecario")]
        public async Task<IActionResult> ExportarExcel()
        {
            var libros = await _context.Libros
                .AsNoTracking()
                .OrderBy(libro => libro.Titulo)
                .ToListAsync();

            using var libroExcel = new XLWorkbook();
            var hoja = libroExcel.Worksheets.Add("Reporte de Libros");

            hoja.Cell("A1").Value = "Reporte de Libros";
            hoja.Range("A1:D1").Merge();
            hoja.Cell("A1").Style.Font.Bold = true;
            hoja.Cell("A1").Style.Font.FontSize = 16;
            hoja.Cell("A1").Style.Font.FontColor = XLColor.White;
            hoja.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
            hoja.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            hoja.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            hoja.Row(1).Height = 28;

            hoja.Cell("A2").Value = "Fecha de generación:";
            hoja.Cell("B2").Value = DateTime.Now;
            hoja.Cell("B2").Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
            hoja.Range("A2:D2").Style.Font.Italic = true;

            var encabezados = new[] { "ID", "Nombre del libro", "Autor", "Categoría" };
            for (var columna = 0; columna < encabezados.Length; columna++)
            {
                var celda = hoja.Cell(4, columna + 1);
                celda.Value = encabezados[columna];
                celda.Style.Font.Bold = true;
                celda.Style.Font.FontColor = XLColor.White;
                celda.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
                celda.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                celda.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            for (var fila = 0; fila < libros.Count; fila++)
            {
                var libro = libros[fila];
                var numeroFila = fila + 5;

                hoja.Cell(numeroFila, 1).Value = libro.Id;
                hoja.Cell(numeroFila, 2).Value = libro.Titulo;
                hoja.Cell(numeroFila, 3).Value = libro.Autor;
                hoja.Cell(numeroFila, 4).Value = libro.Categoria;
            }

            var rangoReporte = hoja.Range(4, 1, Math.Max(4, libros.Count + 4), 4);
            rangoReporte.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangoReporte.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rangoReporte.Style.Border.OutsideBorderColor = XLColor.FromHtml("#B4C7E7");
            rangoReporte.Style.Border.InsideBorderColor = XLColor.FromHtml("#D9E2F3");
            rangoReporte.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            hoja.Range(5, 1, Math.Max(5, libros.Count + 4), 1)
                .Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            hoja.Range(4, 1, Math.Max(4, libros.Count + 4), 4).SetAutoFilter();
            hoja.SheetView.FreezeRows(4);
            hoja.Columns(1, 4).AdjustToContents();
            hoja.Column(1).Width = Math.Max(hoja.Column(1).Width, 8);
            hoja.Column(2).Width = Math.Min(Math.Max(hoja.Column(2).Width, 20), 45);
            hoja.Column(3).Width = Math.Min(Math.Max(hoja.Column(3).Width, 18), 35);
            hoja.Column(4).Width = Math.Min(Math.Max(hoja.Column(4).Width, 18), 35);

            using var memoria = new MemoryStream();
            libroExcel.SaveAs(memoria);

            return File(
                memoria.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Reporte_Libros_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        private static byte[] GenerarReporte(IReadOnlyCollection<Libro> libros)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(document => document.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(42);
                page.PageColor(Colors.Grey.Lighten5);
                page.Header().Column(column =>
                {
                    column.Item().Text("BIBLIOTECA UNIVERSITARIA")
                        .FontSize(11)
                        .Bold()
                        .FontColor(Colors.Blue.Darken2);
                    column.Item().Text("Reporte de libros")
                        .FontSize(26)
                        .Bold()
                        .FontColor(Colors.Grey.Darken3);
                    column.Item().Text($"Listado actualizado el {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Darken1);
                    column.Item().PaddingTop(14).LineHorizontal(2).LineColor(Colors.Blue.Medium);
                });
                page.Content().PaddingVertical(22).Column(column =>
                {
                    column.Item().Background(Colors.Blue.Darken2).Padding(14).Row(row =>
                    {
                        row.RelativeItem().Text("Libros Registrados")
                            .FontSize(12)
                            .Bold()
                            .FontColor(Colors.White);
                        row.ConstantItem(45).AlignRight().Text(libros.Count.ToString())
                            .FontSize(18)
                            .Bold()
                            .FontColor(Colors.White);
                    });

                    column.Item().PaddingTop(18).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(42);
                            columns.RelativeColumn(2.5f);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(1.3f);
                            columns.RelativeColumn(1.3f);
                            columns.RelativeColumn(1.3f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text("ID");
                            header.Cell().Element(HeaderCellStyle).Text("Titulo");
                            header.Cell().Element(HeaderCellStyle).Text("Autor");
                            header.Cell().Element(HeaderCellStyle).Text("Categoria");
                            header.Cell().Element(HeaderCellStyle).Text("Precio");
                            header.Cell().Element(HeaderCellStyle).Text("Disponible");

                        });

                        foreach (var libro in libros)
                        {
                            table.Cell().Element(BodyCellStyle).Text(libro.Id.ToString());
                            table.Cell().Element(BodyCellStyle).Text(libro.Titulo);
                            table.Cell().Element(BodyCellStyle).Text(libro.Autor ?? "No especificado");
                            table.Cell().Element(BodyCellStyle).Text(libro.Categoria ?? "No especificada");
                            table.Cell().Element(BodyCellStyle).Text(libro.Precio.ToString("C"));
                            table.Cell().Element(BodyCellStyle).Text(libro.Disponible ? "Sí" : "No");
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Universidad Evangelica - Pagina ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            })).GeneratePdf();

            static IContainer HeaderCellStyle(IContainer container) => container
           .Background(Colors.Blue.Darken2)
           .PaddingVertical(9)
           .PaddingHorizontal(8)
           .DefaultTextStyle(text => text.FontSize(9).Bold().FontColor(Colors.White));

            static IContainer BodyCellStyle(IContainer container) => container
           .Background(Colors.White)
           .BorderBottom(1)
           .BorderColor(Colors.Grey.Lighten2)
           .PaddingVertical(9)
           .PaddingHorizontal(8)
           .DefaultTextStyle(text => text.FontSize(9).Bold().FontColor(Colors.Grey.Darken3));
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Iniciando consulta del libro con identificador {LibroId}", id);
            try
            {
                var libro = await _context.Libros.FindAsync(id);
                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el libro con identificador {LibroId}", id);
                    return NotFound();
                }
                return View(libro);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al consultar el libro con identificador {LibroId}", id);
                throw;
            }
        }

        [Authorize(Roles = "Admin,Bibliotecario")]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Iniciando formulario de creación de libro");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Bibliotecario")]
        public async Task<IActionResult> Create(Libro libro)
        {
            _logger.LogInformation("Iniciando creación de libro");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("La creación de libro contiene datos no válidos");
                return View(libro);
            }
            try
            {
                _context.Libros.Add(libro);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Libro creado correctamente con identificador {LibroId}", libro.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al crear el libro");
                throw;
            }
        }

        [Authorize(Roles = "Admin,Bibliotecario")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Iniciando formulario de edición del libro con identificador {LibroId}", id);
            try
            {
                var libro = await _context.Libros.FindAsync(id);
                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el libro con identificador {LibroId} para editar", id);
                    return NotFound();
                }
                return View(libro);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al cargar el libro con identificador {LibroId} para editar", id);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Bibliotecario")]
        public async Task<IActionResult> Edit(Libro libro)
        {
            _logger.LogInformation("Iniciando edición del libro con identificador {LibroId}", libro.Id);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("La edición del libro con identificador {LibroId} contiene datos no válidos", libro.Id);
                return View(libro);
            }
            try
            {
                _context.Libros.Update(libro);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Libro actualizado correctamente con identificador {LibroId}", libro.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al editar el libro con identificador {LibroId}", libro.Id);
                throw;
            }
        }

        [Authorize(Roles = "Admin,Bibliotecario")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Iniciando formulario de eliminación del libro con identificador {LibroId}", id);
            try
            {
                var libro = await _context.Libros.FindAsync(id);
                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el libro con identificador {LibroId} para eliminar", id);
                    return NotFound();
                }
                return View(libro);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al cargar el libro con identificador {LibroId} para eliminar", id);
                throw;
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Bibliotecario")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Iniciando eliminación del libro con identificador {LibroId}", id);
            try
            {
                var libro = await _context.Libros.FindAsync(id);
                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el libro con identificador {LibroId} para eliminar", id);
                    return NotFound();
                }
                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Libro eliminado correctamente con identificador {LibroId}", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al eliminar el libro con identificador {LibroId}", id);
                throw;
            }
        }
    }
}
