using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class Libro
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }
        [Required]
        [StringLength(100)]
        public string Autor { get; set; }
        [Required]
        [StringLength(100)]
        public string Categoria { get; set; }
        [Required]
        public decimal Precio { get; set; }
        [Required]
        public bool Disponible { get; set; }
    }
}
