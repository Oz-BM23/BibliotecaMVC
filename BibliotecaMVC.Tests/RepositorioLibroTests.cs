using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;

namespace BibliotecaMVC.Tests;

public class RepositorioLibroTests
{
    [Fact]
    public void ObtenerTodo_ConRegistros_DevuelveLosLibrosIniciales()
    {
        var repository = new RepositorioenMemoria();

        var libros = repository.ObtenerTodo().ToList();

        Assert.Equal(5, libros.Count);
        Assert.Contains(libros, libro => libro.Titulo == "Cien Años de Soledad");
    }

    [Fact]
    public void ObtenerPorId_ConLibroExistente_DevuelveElLibroEsperado()
    {
        var repository = new RepositorioenMemoria();

        var libro = repository.ObtenerPorId(1);

        Assert.NotNull(libro);
        Assert.Equal(1, libro.Id);
        Assert.Equal("Cien Años de Soledad", libro.Titulo);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public void ObtenerPorId_ConIdInexistente_DevuelveNulo(int id)
    {
        var repository = new RepositorioenMemoria();

        var libro = repository.ObtenerPorId(id);

        Assert.Null(libro);
    }

    [Fact]
    public void Agregar_ConLibroValido_AsignaIdYLoIncluyeEnElListado()
    {
        var repository = new RepositorioenMemoria();
        var libro = CrearLibro("Libro nuevo");

        repository.Agregar(libro);

        Assert.Equal(6, libro.Id);
        Assert.Same(libro, repository.ObtenerPorId(6));
        Assert.Equal(6, repository.ObtenerTodo().Count());
    }

    [Fact]
    public void Actualizar_ConLibroExistente_ReemplazaSusDatosYDevuelveTrue()
    {
        var repository = new RepositorioenMemoria();
        var libroActualizado = CrearLibro("Título actualizado");
        libroActualizado.Id = 2;

        var resultado = repository.Actualizar(libroActualizado);

        Assert.True(resultado);
        var libro = repository.ObtenerPorId(2);
        Assert.NotNull(libro);
        Assert.Equal("Título actualizado", libro.Titulo);
    }

    [Fact]
    public void Actualizar_ConLibroInexistente_DevuelveFalseYSinCambiarLaCantidad()
    {
        var repository = new RepositorioenMemoria();
        var libro = CrearLibro("Libro inexistente");
        libro.Id = 999;

        var resultado = repository.Actualizar(libro);

        Assert.False(resultado);
        Assert.Equal(5, repository.ObtenerTodo().Count());
        Assert.Null(repository.ObtenerPorId(999));
    }

    [Fact]
    public void Eliminar_ConLibroExistente_LoEliminaYDevuelveTrue()
    {
        var repository = new RepositorioenMemoria();

        var resultado = repository.Eliminar(3);

        Assert.True(resultado);
        Assert.Null(repository.ObtenerPorId(3));
        Assert.Equal(4, repository.ObtenerTodo().Count());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public void Eliminar_ConLibroInexistente_DevuelveFalseYSinEliminarRegistros(int id)
    {
        var repository = new RepositorioenMemoria();

        var resultado = repository.Eliminar(id);

        Assert.False(resultado);
        Assert.Equal(5, repository.ObtenerTodo().Count());
    }

    private static Libro CrearLibro(string titulo)
    {
        return new Libro
        {
            Titulo = titulo,
            Autor = "Autor de prueba",
            Categoria = "Categoría de prueba",
            Precio = 12.50m,
            Disponible = true
        };
    }
}
