// Importa las clases que permiten ejecutar las validaciones de Data Annotations.
using System.ComponentModel.DataAnnotations;
// Importa el modelo Autor de la aplicación principal.
using BibliotecaMVC.Models;

// Define el espacio de nombres del proyecto de pruebas.
namespace BibliotecaMVC.Tests;

// Agrupa las pruebas relacionadas con las reglas de validación de Autor.
public class AutorValidationTests
{
    // Indica a xUnit que el siguiente método es una prueba.
    [Fact]
    // Comprueba que el nombre del autor sea obligatorio.
    public void NombreEsObligatorio()
    {
        // Crea un autor con el nombre vacío para provocar la validación requerida.
        var autor = new Autor { Nombre = string.Empty };

        // Ejecuta las validaciones definidas mediante atributos en Autor.
        var errores = Validate(autor);

        // Comprueba que exista un error asociado a la propiedad Nombre.
        Assert.Contains(errores, error => error.MemberNames.Contains(nameof(Autor.Nombre)));
    }

    // Indica a xUnit que el siguiente método es una prueba.
    [Fact]
    // Comprueba que el nombre no supere los 100 caracteres.
    public void NombreNoPuedeSuperarLos100Caracteres()
    {
        // Crea un nombre con 101 caracteres para comprobar el límite máximo.
        var autor = new Autor { Nombre = new string('A', 101) };

        // Ejecuta las validaciones del modelo.
        var errores = Validate(autor);

        // Comprueba que el error corresponda a la propiedad Nombre.
        Assert.Contains(errores, error => error.MemberNames.Contains(nameof(Autor.Nombre)));
    }

    // Indica a xUnit que el siguiente método es una prueba.
    [Fact]
    // Comprueba que el límite exacto de 100 caracteres sea válido.
    public void NombreCon100CaracteresEsValido()
    {
        // Crea un nombre con exactamente 100 caracteres.
        var autor = new Autor { Nombre = new string('A', 100) };

        // Ejecuta las validaciones del modelo.
        var errores = Validate(autor);

        // Comprueba que no haya errores para la propiedad Nombre.
        Assert.DoesNotContain(errores, error => error.MemberNames.Contains(nameof(Autor.Nombre)));
    }

    // Indica a xUnit que el siguiente método es una prueba.
    [Fact]
    // Comprueba que la nacionalidad no supere los 50 caracteres.
    public void NacionalidadNoPuedeSuperarLos50Caracteres()
    {
        // Crea un autor con un nombre válido y una nacionalidad demasiado larga.
        var autor = new Autor
        {
            // Asigna un nombre válido para que el resultado se centre en Nacionalidad.
            Nombre = "Autor válido",
            // Genera una nacionalidad de 51 caracteres, superando el máximo permitido.
            Nacionalidad = new string('B', 51)
        };

        // Ejecuta las validaciones del modelo.
        var errores = Validate(autor);

        // Comprueba que el error corresponda a la propiedad Nacionalidad.
        Assert.Contains(errores, error => error.MemberNames.Contains(nameof(Autor.Nacionalidad)));
    }

    // Indica a xUnit que el siguiente método es una prueba.
    [Fact]
    // Comprueba que el nombre acepte caracteres especiales y tildes.
    public void NombreConCaracteresEspecialesEsValido()
    {
        // Crea un autor cuyo nombre contiene tildes y varios nombres.
        var autor = new Autor { Nombre = "José María de la Cruz" };

        // Ejecuta las validaciones del modelo.
        var errores = Validate(autor);

        // Comprueba que el autor no tenga ningún error de validación.
        Assert.Empty(errores);
    }

    // Método auxiliar que concentra la ejecución de las validaciones del modelo.
    private static IReadOnlyCollection<ValidationResult> Validate(Autor autor)
    {
        // Crea el contexto de validación para la instancia recibida.
        var context = new ValidationContext(autor);
        // Prepara la colección donde se almacenarán los errores encontrados.
        var errores = new List<ValidationResult>();
        // Ejecuta todas las validaciones, incluso las propiedades no obligatorias.
        Validator.TryValidateObject(autor, context, errores, validateAllProperties: true);
        // Devuelve los errores para que cada prueba pueda analizarlos.
        return errores;
    }
}
