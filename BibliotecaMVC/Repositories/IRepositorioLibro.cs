using BibliotecaMVC.Models;

namespace BibliotecaMVC.Repositories
{
    public interface IRepositorioLibro
    {
        IEnumerable<Libro> ObtenerTodo();
        Libro? ObtenerPorId(int id);
        void Agregar(Libro libro);
        bool Actualizar(Libro libro);
        bool Eliminar(int id);

    }
}
