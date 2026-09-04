namespace BibliotecaMVC.Models
{
    public class Autor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Nacionalidad { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public bool Activo { get; set; }
    }
}