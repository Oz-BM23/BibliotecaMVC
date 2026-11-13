using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class AutoresController : Controller
    {
        private readonly BibliotecaContext _context;

        public AutoresController(BibliotecaContext context, ILogger<AutoresController> @object)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var autores = await _context.Autores.ToListAsync();
            return View(autores);

        }

        public async Task<IActionResult> Reporte()
        {
            var autores = await _context.Autores
                .AsNoTracking()
                .OrderBy(autor => autor.Nombre)
                .ToListAsync();

            return File(GenerarReporte(autores), "application/pdf");
        }

        public async Task<IActionResult> DescargarReporte()
        {
            var autores = await _context.Autores
                .AsNoTracking()
                .OrderBy(autor => autor.Nombre)
                .ToListAsync();

            return File(GenerarReporte(autores), "application/pdf", "Reporte_Autores.pdf");
        }

        private static byte[] GenerarReporte(IReadOnlyCollection<Autor> autores)
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
                    column.Item().Text("Reporte de autores")
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
                        row.RelativeItem().Text("Autores Registrados")
                            .FontSize(12)
                            .Bold()
                            .FontColor(Colors.White);
                        row.ConstantItem(45).AlignRight().Text(autores.Count.ToString())
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
                            header.Cell().Element(HeaderCellStyle).Text("Nombre");
                            header.Cell().Element(HeaderCellStyle).Text("Apellido");
                            header.Cell().Element(HeaderCellStyle).Text("Nacionalidad");
                            header.Cell().Element(HeaderCellStyle).Text("Fecha de Nacimiento"); 
                            header.Cell().Element(HeaderCellStyle).Text("Activo");

                        });

                        foreach (var autor in autores)
                        {
                            table.Cell().Element(BodyCellStyle).Text(autor.Id.ToString());
                            table.Cell().Element(BodyCellStyle).Text(autor.Nombre);
                            table.Cell().Element(BodyCellStyle).Text(autor.Apellido);
                            table.Cell().Element(BodyCellStyle).Text(autor.Nacionalidad ?? "No especificada");
                            table.Cell().Element(BodyCellStyle).Text(autor.FechaNacimiento.ToString("dd/MM/yyyy") ?? "No especificada");
                            table.Cell().Element(BodyCellStyle).Text(autor.Activo ? "Sí" : "No");
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
            var autor = await _context.Autores.FindAsync(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }
        [Authorize(Roles = "Admin, Bibliotecario")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Create(Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }
            
            _context.Autores.Add(autor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); 
        }

        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Edit(int id)
        {
            var autor = await _context.Autores.FindAsync(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Edit(int id, Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }

            _context.Autores.Update(autor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); 
        }

        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> Delete(int id)
        {
            var autor = await _context.Autores.FindAsync(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Bibliotecario")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var autor = await _context.Autores.FindAsync(id);
            if (autor == null)
            {
                return NotFound();
            }

            _context.Autores.Remove(autor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
