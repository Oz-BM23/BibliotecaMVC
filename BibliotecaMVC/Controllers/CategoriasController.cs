using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Authorization;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(IConfiguration configuration, ILogger<CategoriasController> logger)
        {
            _connectionString = configuration.GetConnectionString("BibliotecaDB");
            _logger = logger;
        }
        public IActionResult Index()
        {
            _logger.LogInformation("Iniciando consulta de categorías");
            var categorias = new List<Categoria>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var SQL = "SELECT ID, Nombre, Descripcion FROM Categorias";
                    using (var comando = new SqlCommand(SQL, connection))
                    {
                        connection.Open();
                        using (var lector = comando.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                var categoria = new Categoria
                                {
                                    ID = lector.GetInt32(0),
                                    Nombre = lector.GetString(1),
                                    Descripcion = lector.IsDBNull(2) ? null : lector.GetString(2)
                                };
                                categorias.Add(categoria);
                            }
                        }
                    }
                }
                _logger.LogInformation("Consulta de categorías completada");
                return View(categorias);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al consultar las categorías");
                throw;
            }
        }
        public IActionResult Create()
        {
            _logger.LogInformation("Iniciando formulario de creación de categoría");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria categoria)
        {
            _logger.LogInformation("Iniciando creación de categoría");
            if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                _logger.LogWarning("La creación de categoría contiene datos no válidos");
                ModelState.AddModelError("Nombre", "El nombre es obligatorio.");
                return View(categoria);
            }
            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "INSERT INTO Categorias (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";
                    using (var comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                        comando.Parameters.AddWithValue("@Descripcion", (object)categoria.Descripcion ?? System.DBNull.Value);
                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }
                _logger.LogInformation("Categoría creada correctamente");
                TempData["SuccessMessage"] = "Categoria creada exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al crear la categoría");
                throw;
            }
        }
        public IActionResult Edit(int id)
        {
            _logger.LogInformation("Iniciando formulario de edición de la categoría con identificador {CategoriaId}", id);
            Categoria categoria = null;
            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "SELECT ID, Nombre, Descripcion FROM Categorias WHERE ID = @ID";

                    using (var cmd = new SqlCommand(sql, conexion))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        conexion.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                categoria = new Categoria
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    Nombre = reader["Nombre"].ToString(),
                                    Descripcion = reader["Descripcion"].ToString()
                                };
                            }
                        }
                    }
                }
                if (categoria == null)
                {
                    _logger.LogWarning("No se encontró la categoría con identificador {CategoriaId} para editar", id);
                    return NotFound();
                }
                return View(categoria);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al cargar la categoría con identificador {CategoriaId} para editar", id);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria categoria)
        {
            _logger.LogInformation("Iniciando edición de la categoría con identificador {CategoriaId}", categoria?.ID);
            if (categoria == null || string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                _logger.LogWarning("La edición de categoría contiene datos no válidos");
                ModelState.AddModelError("Nombre", "El nombre es obligatorio.");
                return View(categoria);
            }
            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "UPDATE Categorias SET Nombre = @Nombre, Descripcion = @Descripcion WHERE ID = @ID";

                    using (var cmd = new SqlCommand(sql, conexion))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                        cmd.Parameters.AddWithValue("@Descripcion", (object)categoria.Descripcion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ID", categoria.ID);

                        conexion.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                _logger.LogInformation("Categoría actualizada correctamente con identificador {CategoriaId}", categoria.ID);
                TempData["SuccessMessage"] = "Categoria actualizada exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al editar la categoría con identificador {CategoriaId}", categoria.ID);
                throw;
            }
        }
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Iniciando formulario de eliminación de la categoría con identificador {CategoriaId}", id);
            Categoria categoria = null;
            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "SELECT ID, Nombre, Descripcion FROM Categorias WHERE ID = @ID";

                    using (var cmd = new SqlCommand(sql, conexion))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        conexion.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                categoria = new Categoria
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    Nombre = reader["Nombre"].ToString(),
                                    Descripcion = reader["Descripcion"].ToString()
                                };
                            }
                        }
                    }
                }
                if (categoria == null)
                {
                    _logger.LogWarning("No se encontró la categoría con identificador {CategoriaId} para eliminar", id);
                    return NotFound();
                }
                return View(categoria);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al cargar la categoría con identificador {CategoriaId} para eliminar", id);
                throw;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _logger.LogInformation("Iniciando eliminación de la categoría con identificador {CategoriaId}", id);
            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    var sql = "DELETE FROM Categorias WHERE ID = @ID";

                    using (var cmd = new SqlCommand(sql, conexion))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);

                        conexion.Open();
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            _logger.LogWarning("No se encontró la categoría con identificador {CategoriaId} para eliminar", id);
                            return NotFound();
                        }
                    }
                }
                _logger.LogInformation("Categoría eliminada correctamente con identificador {CategoriaId}", id);
                TempData["SuccessMessage"] = "Categoria eliminada exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error al eliminar la categoría con identificador {CategoriaId}", id);
                throw;
            }
        }

    }
}