using BibliotecaMVC.Models;

namespace BibliotecaMVC.Repositories

{
    public interface IAutorService
    {
        IEnumerable<Autor> ObtenerTodo();
        Autor? ObtenerPorId(int id);
        void Agregar(Autor autor);
        bool Actualizar(Autor autor);
        bool Eliminar(int id);
    }
}