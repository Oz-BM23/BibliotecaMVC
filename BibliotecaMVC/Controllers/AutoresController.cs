using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        private readonly IAutorService servicio;

        public AutoresController(IAutorService servicio)
        {
            this.servicio = servicio;
        }

        public IActionResult Index()
        {
            var autores = servicio.ObtenerTodo();
            return View(autores);
        }

        public IActionResult Details(int id)
        {
            var autor = servicio.ObtenerPorId(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }
            servicio.Agregar(autor);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var autor = servicio.ObtenerPorId(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Autor autor)
        {
            if (!ModelState.IsValid)
            {
                return View(autor);
            }

            var actualizado = servicio.Actualizar(autor);
            if (!actualizado)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var autor = servicio.ObtenerPorId(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var eliminado = servicio.Eliminar(id);
            if (!eliminado)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}