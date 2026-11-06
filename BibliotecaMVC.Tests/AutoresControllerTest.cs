using BibliotecaMVC.Controllers;
using BibliotecaMVC.Models;
using BibliotecaMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BibliotecaMVC.Tests;

public class AutoresControllerTests
{
    [Fact]
    // Comprueba que Index devuelva todos los autores guardados.
    public async Task Index_DevuelveLaListaDeAutores()
    {
        // Crea un contexto EF Core con una base de datos en memoria única.
        await using var context = CreateContext();
        // Inserta dos autores de prueba en el contexto.
        context.Autores.AddRange(
            new Autor { Nombre = "Gabriel García Márquez", Nacionalidad = "Colombiana" },
            new Autor { Nombre = "Isabel Allende", Nacionalidad = "Chilena" });
        // Persiste los autores en la base de datos en memoria.
        await context.SaveChangesAsync();
        // Crea el controlador usando el contexto y un logger simulado.
        var controller = CreateController(context);

        // Ejecuta la acción Index del controlador.
        var result = await controller.Index();

        // Comprueba que el resultado sea una vista.
        var view = Assert.IsType<ViewResult>(result);
        // Obtiene el modelo de la vista como una colección de autores.
        var autores = Assert.IsAssignableFrom<IEnumerable<Autor>>(view.Model);
        // Comprueba que la vista contenga los dos autores insertados.
        Assert.Equal(2, autores.Count());
    }



    // Indica a xUnit que el siguiente método es una prueba.
    [Fact]
    // Comprueba que Details devuelva un autor existente.
    public async Task Details_ConAutorExistente_DevuelveElAutor()
    {
        // Crea una base de datos en memoria aislada.
        await using var context = CreateContext();
        // Crea un autor de prueba.
        var autor = new Autor { Nombre = "Julio Cortázar", Nacionalidad = "Argentina" };
        // Añade el autor al contexto.
        context.Autores.Add(autor);
        // Guarda el autor y genera su identificador.
        await context.SaveChangesAsync();
        // Crea el controlador de prueba.
        var controller = CreateController(context);

        // Solicita los detalles usando el identificador generado.
        var result = await controller.Details(autor.Id);

        // Comprueba que la respuesta sea una vista.
        var view = Assert.IsType<ViewResult>(result);
        // Comprueba que la vista reciba exactamente el autor guardado.
        Assert.Same(autor, view.Model);
    }


    [Fact]
    // Comprueba la respuesta cuando el autor no existe.
    public async Task Details_ConAutorInexistente_DevuelveNotFound()
    {
        // Crea un contexto vacío.
        await using var context = CreateContext();
        // Crea el controlador de prueba.
        var controller = CreateController(context);

        // Solicita los detalles de un identificador inexistente.
        var result = await controller.Details(999);

        // Comprueba que el controlador responda con NotFound.
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    // Comprueba que Create guarde un autor válido y redirija al listado.
    public async Task Create_ConAutorValido_GuardaElAutorYRedirigeAlIndice()
    {
        // Crea una base de datos en memoria vacía.
        await using var context = CreateContext();
        // Crea el controlador de prueba.
        var controller = CreateController(context);
        // Construye el autor válido que se enviará a la acción.
        var autor = new Autor { Nombre = "Mario Vargas Llosa", Nacionalidad = "Peruana" };

        // Ejecuta la acción Create con el autor.
        var result = await controller.Create(autor);

        // Comprueba que la acción devuelva una redirección.
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        // Comprueba que la redirección apunte a Index.
        Assert.Equal(nameof(AutoresController.Index), redirect.ActionName);
        // Comprueba que se haya guardado un único autor.
        Assert.Equal(1, await context.Autores.CountAsync());
        // Comprueba que el autor guardado tenga el nombre esperado.
        Assert.Equal("Mario Vargas Llosa", (await context.Autores.SingleAsync()).Nombre);
    }

    [Fact]
    // Comprueba que Create no guarde datos cuando el modelo es inválido.
    public async Task Create_ConModeloInvalido_NoGuardaYDevuelveLaVista()
    {
        // Crea una base de datos en memoria vacía.
        await using var context = CreateContext();
        // Crea el controlador de prueba.
        var controller = CreateController(context);
        // Simula un error de validación del nombre.
        controller.ModelState.AddModelError(nameof(Autor.Nombre), "El nombre es obligatorio.");
        // Crea el autor que se enviará con el modelo inválido.
        var autor = new Autor { Nacionalidad = "Chilena" };

        // Ejecuta la acción Create.
        var result = await controller.Create(autor);

        // Comprueba que se devuelva nuevamente la vista.
        var view = Assert.IsType<ViewResult>(result);
        // Comprueba que la vista conserve el modelo enviado.
        Assert.Same(autor, view.Model);
        // Comprueba que no se haya guardado ningún autor.
        Assert.Empty(context.Autores);
    }

    [Fact]
    // Comprueba que Edit actualice los datos de un autor existente.
    public async Task Edit_ConAutorValido_ActualizaElAutor()
    {
        // Genera un nombre único para la base de datos de esta prueba.
        var databaseName = Guid.NewGuid().ToString();
        // Declara la variable donde se almacenará el identificador del autor.
        int autorId;
        // Crea un contexto separado para preparar los datos iniciales.
        await using (var seedContext = CreateContext(databaseName))
        {
            // Crea el autor original.
            var autor = new Autor { Nombre = "Nombre original", Nacionalidad = "Española" };
            // Añade el autor al contexto inicial.
            seedContext.Autores.Add(autor);
            // Guarda el autor en la base de datos en memoria.
            await seedContext.SaveChangesAsync();
            // Conserva el identificador generado.
            autorId = autor.Id;
        }

        // Crea otro contexto para simular una nueva petición HTTP.
        await using var context = CreateContext(databaseName);
        // Crea el controlador con el contexto de la petición.
        var controller = CreateController(context);
        // Crea la versión modificada del autor.
        var autorEditado = new Autor
        {
            // Mantiene el identificador original.
            Id = autorId,
            // Define el nuevo nombre.
            Nombre = "Nombre actualizado",
            // Mantiene la nacionalidad.
            Nacionalidad = "Española"
        };

        // Ejecuta la acción Edit.
        var result = await controller.Edit(autorId, autorEditado);

        // Comprueba que la acción redirija después de actualizar.
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        // Comprueba que la redirección vaya al listado.
        Assert.Equal(nameof(AutoresController.Index), redirect.ActionName);
        // Obtiene el autor sin seguimiento para comprobar el valor persistido.
        var autorGuardado = await context.Autores.AsNoTracking().SingleAsync();
        // Comprueba que el nombre se haya actualizado.
        Assert.Equal("Nombre actualizado", autorGuardado.Nombre);
    }

    [Fact]
    // Comprueba que Edit rechace un identificador distinto al del modelo.
    public async Task Edit_ConIdentificadorDiferente_DevuelveBadRequest()
    {
        // Crea un contexto en memoria.
        await using var context = CreateContext();
        // Crea el controlador de prueba.
        var controller = CreateController(context);
        // Crea un autor cuyo Id no coincide con el parámetro de la acción.
        var autor = new Autor { Id = 1, Nombre = "Autor" };

        // Ejecuta Edit con dos identificadores diferentes.
        var result = await controller.Edit(2, autor);

        // Comprueba que la respuesta sea BadRequest.
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    // Comprueba que Edit devuelva NotFound si el autor no existe.
    public async Task Edit_ConAutorInexistente_DevuelveNotFound()
    {
        // Crea un contexto vacío.
        await using var context = CreateContext();
        // Crea el controlador de prueba.
        var controller = CreateController(context);
        // Construye un autor con un identificador inexistente.
        var autor = new Autor { Id = 999, Nombre = "Autor" };

        // Ejecuta Edit con el identificador inexistente.
        var result = await controller.Edit(999, autor);

        // Comprueba que la respuesta sea NotFound.
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    // Comprueba que Delete elimine un autor existente y redirija al listado.
    public async Task Delete_ConAutorExistente_EliminaElAutorYRedirigeAlIndice()
    {
        // Crea una base de datos en memoria.
        await using var context = CreateContext();
        // Crea el autor que se eliminará.
        var autor = new Autor { Nombre = "Autor para eliminar", Nacionalidad = "Mexicana" };
        // Añade el autor al contexto.
        context.Autores.Add(autor);
        // Persiste el autor y genera su identificador.
        await context.SaveChangesAsync();
        // Crea el controlador de prueba.
        var controller = CreateController(context);

        // Ejecuta Delete con el identificador del autor.
        var result = await controller.Delete(autor.Id);

        // Comprueba que la acción redirija después de eliminar.
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        // Comprueba que la redirección apunte a Index.
        Assert.Equal(nameof(AutoresController.Index), redirect.ActionName);
        // Comprueba que ya no queden autores en la base de datos.
        Assert.Empty(context.Autores);
    }

    [Fact]
    // Comprueba que Delete devuelva NotFound si el autor no existe.
    public async Task Delete_ConAutorInexistente_DevuelveNotFound()
    {
        // Crea un contexto vacío.
        await using var context = CreateContext();
        // Crea el controlador de prueba.
        var controller = CreateController(context);

        // Ejecuta Delete con un identificador inexistente.
        var result = await controller.Delete(999);

        // Comprueba que la respuesta sea NotFound.
        Assert.IsType<NotFoundResult>(result);
    }

    // Crea un controlador usando el contexto indicado y un logger simulado.
    private static AutoresController CreateController(BibliotecaContext context)
    {
        // Crea un mock para no depender de un sistema de logs real.
        var logger = new Mock<ILogger<AutoresController>>();
        // Devuelve una instancia del controlador preparada para la prueba.
        return new AutoresController(context, logger.Object);
    }

    // Crea un contexto conectado a una base de datos EF Core en memoria.
    private static BibliotecaContext CreateContext(string? databaseName = null)
    {
        // Configura las opciones del contexto.
        var options = new DbContextOptionsBuilder<BibliotecaContext>()
            // Usa el nombre recibido o genera uno nuevo para aislar la prueba.
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            // Finaliza la construcción de opciones.
            .Options;

        // Devuelve el contexto configurado.
        return new BibliotecaContext(options);
    }
}
