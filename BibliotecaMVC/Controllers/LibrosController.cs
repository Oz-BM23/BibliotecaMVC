using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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
