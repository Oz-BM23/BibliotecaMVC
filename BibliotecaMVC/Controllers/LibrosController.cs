using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly IRepositorioLibro repositorio;

        public LibrosController(IRepositorioLibro repositorio)
        {
            this.repositorio = repositorio;
        }

        public IActionResult Index()
        {
            var libros = repositorio.ObtenerTodo();
            return View(libros);
        }

        public IActionResult Details(int id)
        {
            var libro = repositorio.ObtenerPorId(id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }
            repositorio.Agregar(libro);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var libro = repositorio.ObtenerPorId(id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Libro libro)
        {
            if (!ModelState.IsValid)
            {
                return View(libro);
            }

            var actualizado = repositorio.Actualizar(libro);
            if (!actualizado)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var libro = repositorio.ObtenerPorId(id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var eliminado = repositorio.Eliminar(id);
            if (!eliminado)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}