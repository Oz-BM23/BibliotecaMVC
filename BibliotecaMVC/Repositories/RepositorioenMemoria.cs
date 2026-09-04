using System.Collections.Generic;
using System.Linq;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Repositories
{
    public class RepositorioenMemoria : IRepositorioLibro
    {
        private readonly List<Libro> _libros;

        public RepositorioenMemoria()
        {
            _libros = new List<Libro>()
            {
                new Libro { Id = 1, Titulo = "Cien Años de Soledad", Autor = "Gabriel García Márquez", Categoria = "Novela", Precio = 19.99m, Disponible = true },
                new Libro { Id = 2, Titulo = "Don Quijote de la Mancha", Autor = "Miguel de Cervantes", Categoria = "Novela", Precio = 14.99m, Disponible = true },
                new Libro { Id = 3, Titulo = "El Principito", Autor = "Antoine de Saint-Exupéry", Categoria = "Infantil", Precio = 9.99m, Disponible = false },
                new Libro { Id = 4, Titulo = "1984", Autor = "George Orwell", Categoria = "Ciencia Ficción", Precio = 12.99m, Disponible = true },
                new Libro { Id = 5, Titulo = "La Odisea", Autor = "Homero", Categoria = "Épica", Precio = 11.99m, Disponible = false },
            };
        }

        public IEnumerable<Libro> ObtenerTodo()
        {
            return _libros;
        }

        public Libro? ObtenerPorId(int id)
        {
            return _libros.FirstOrDefault(l => l.Id == id);
        }

        public void Agregar(Libro libro)
        {
            var nextId = _libros.Any() ? _libros.Max(l => l.Id) + 1 : 1;
            libro.Id = nextId;
            _libros.Add(libro);
        }

        public bool Actualizar(Libro libro)
        {
            var index = _libros.FindIndex(l => l.Id == libro.Id);
            if (index == -1) return false;
            _libros[index] = libro;
            return true;
        }

        public bool Eliminar(int id)
        {
            var libro = _libros.FirstOrDefault(l => l.Id == id);
            if (libro == null) return false;
            return _libros.Remove(libro);
        }
    }
}