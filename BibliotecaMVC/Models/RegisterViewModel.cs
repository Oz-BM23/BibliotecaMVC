using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [Display(Name = "Nombre de usuario")]
        public string? Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        [Display(Name = "Correo electrónico")]
        public string? Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string? Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "La confirmación de la contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string? ConfirmPassword { get; set; } = string.Empty;
    }
}
