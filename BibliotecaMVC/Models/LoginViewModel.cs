using System.ComponentModel.DataAnnotations;    

namespace BibliotecaMVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Nombre de usuario o correo electrónico")]
        public string NameorEmail { get; set; } = string.Empty;
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Display(Name = "Recordarme")]
        public bool RememberMe
        {
            get; set;
        }
        public string? ReturnUrl { get; set; }
    }
}
