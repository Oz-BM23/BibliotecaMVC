using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;

namespace BibliotecaMVC.Services
{
    public class AutorService : IAutorService
    {
        private readonly List<Autor> autores;

        public AutorService()
        {
            autores = new List<Autor>
            {
                new Autor { Id = 1, Nombre = "Gabriel", Apellido = "García Márquez", Nacionalidad = "Colombiana", FechaNacimiento = new DateTime(1928, 3, 6), Activo = true },
                new Autor { Id = 2, Nombre = "Miguel", Apellido = "de Cervantes", Nacionalidad = "Española", FechaNacimiento = new DateTime(1547, 9, 29), Activo = true },
                new Autor { Id = 3, Nombre = "Antoine", Apellido = "de Saint-Exupéry", Nacionalidad = "Francesa", FechaNacimiento = new DateTime(1900, 6, 29), Activo = false },
                new Autor { Id = 4, Nombre = "George", Apellido = "Orwell", Nacionalidad = "Británica", FechaNacimiento = new DateTime(1903, 6, 25), Activo = true },
                new Autor { Id = 5, Nombre = "Jane", Apellido = "Austen", Nacionalidad = "Británica", FechaNacimiento = new DateTime(1775, 12, 16), Activo = false }
            };
        }

        public IEnumerable<Autor> ObtenerTodo()
        {
            return autores;
        }

        public Autor? ObtenerPorId(int id)
        {
            return autores.FirstOrDefault(a => a.Id == id);
        }

        public void Agregar(Autor autor)
        {
            var nextId = autores.Any() ? autores.Max(a => a.Id) + 1 : 1;
            autor.Id = nextId;
            autores.Add(autor);
        }

        public bool Actualizar(Autor autor)
        {
            var index = autores.FindIndex(a => a.Id == autor.Id);
            if (index == -1) return false;
            autores[index] = autor;
            return true;
        }

        public bool Eliminar(int id)
        {
            var autor = autores.FirstOrDefault(a => a.Id == id);
            if (autor == null) return false;
            return autores.Remove(autor);
        }
    }
}
