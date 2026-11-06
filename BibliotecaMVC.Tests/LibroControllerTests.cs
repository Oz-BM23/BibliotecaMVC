using BibliotecaMVC.Controllers;
using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace BibliotecaMVC.Tests;

public class LibroControllerTests
{
    [Fact]
    public async Task Index_ConLibros_DevuelveTodosLosLibros()
    {
        await using var context = CreateContext();
        context.Libros.AddRange(
            LibroValido("Cien años de soledad"),
            LibroValido("El principito"));
        await context.SaveChangesAsync();
        var controller = CreateController(context);

        var result = await controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        var libros = Assert.IsAssignableFrom<IEnumerable<Libro>>(view.Model);
        Assert.Equal(2, libros.Count());
        Assert.Contains(libros, libro => libro.Titulo == "Cien años de soledad");
    }

    [Fact]
    public async Task Index_SinLibros_DevuelveColeccionVacia()
    {
        await using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        var libros = Assert.IsAssignableFrom<IEnumerable<Libro>>(view.Model);
        Assert.Empty(libros);
    }

    [Fact]
    public async Task Details_ConLibroExistente_DevuelveElLibro()
    {
        await using var context = CreateContext();
        var libro = LibroValido("1984");
        context.Libros.Add(libro);
        await context.SaveChangesAsync();
        var controller = CreateController(context);

        var result = await controller.Details(libro.Id);

        var view = Assert.IsType<ViewResult>(result);
        var libroDevuelto = Assert.IsType<Libro>(view.Model);
        Assert.NotNull(libroDevuelto);
        Assert.Equal("1984", libroDevuelto.Titulo);
        Assert.Equal(libro.Id, libroDevuelto.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public async Task Details_ConIdentificadorInexistente_DevuelveNotFound(int id)
    {
        await using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.Details(id);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ConLibroValido_GuardaElLibroYRedirigeAlIndice()
    {
        await using var context = CreateContext();
        var controller = CreateController(context);
        var libro = LibroValido("La Odisea");

        var result = await controller.Create(libro);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        Assert.Equal(1, await context.Libros.CountAsync());
        Assert.Equal("La Odisea", (await context.Libros.SingleAsync()).Titulo);
        Assert.True(libro.Id > 0);
    }

    [Fact]
    public async Task Create_ConModeloInvalido_NoGuardaYDevuelveLaVista()
    {
        await using var context = CreateContext();
        var controller = CreateController(context);
        controller.ModelState.AddModelError(nameof(Libro.Titulo), "El título es obligatorio.");
        var libro = LibroValido(string.Empty);

        var result = await controller.Create(libro);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(libro, view.Model);
        Assert.Empty(context.Libros);
    }

    [Fact]
    public async Task Edit_ConLibroValido_ActualizaElLibroYRedirigeAlIndice()
    {
        await using var context = CreateContext();
        var libro = LibroValido("Título original");
        context.Libros.Add(libro);
        await context.SaveChangesAsync();
        var id = libro.Id;
        context.Entry(libro).State = EntityState.Detached;
        var controller = CreateController(context);
        var libroEditado = LibroValido("Título actualizado");
        libroEditado.Id = id;

        var result = await controller.Edit(libroEditado);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        var guardado = await context.Libros.AsNoTracking().SingleAsync();
        Assert.Equal(id, guardado.Id);
        Assert.Equal("Título actualizado", guardado.Titulo);
    }

    [Fact]
    public async Task Edit_ConModeloInvalido_NoActualizaYDevuelveLaVista()
    {
        await using var context = CreateContext();
        var original = LibroValido("Título original");
        context.Libros.Add(original);
        await context.SaveChangesAsync();
        var controller = CreateController(context);
        controller.ModelState.AddModelError(nameof(Libro.Titulo), "El título es obligatorio.");
        var libroEditado = LibroValido(string.Empty);
        libroEditado.Id = original.Id;

        var result = await controller.Edit(libroEditado);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(libroEditado, view.Model);
        Assert.Equal("Título original", (await context.Libros.AsNoTracking().SingleAsync()).Titulo);
    }

    [Fact]
    public async Task DeleteConfirmed_ConLibroExistente_EliminaElLibroYRedirigeAlIndice()
    {
        await using var context = CreateContext();
        var libro = LibroValido("Libro para eliminar");
        context.Libros.Add(libro);
        await context.SaveChangesAsync();
        var controller = CreateController(context);

        var result = await controller.DeleteConfirmed(libro.Id);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        Assert.Empty(context.Libros);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public async Task DeleteConfirmed_ConLibroInexistente_DevuelveNotFound(int id)
    {
        await using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.DeleteConfirmed(id);

        Assert.IsType<NotFoundResult>(result);
        Assert.Empty(context.Libros);
    }

    private static LibrosController CreateController(BibliotecaContext context)
    {
        var logger = new Mock<ILogger<LibrosController>>();
        return new LibrosController(context, logger.Object);
    }

    private static BibliotecaContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<BibliotecaContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new BibliotecaContext(options);
    }

    private static Libro LibroValido(string titulo)
    {
        return new Libro
        {
            Titulo = titulo,
            Autor = "Autor de prueba",
            Categoria = "Categoría de prueba",
            Precio = 10.00m,
            Disponible = true
        };
    }
}
