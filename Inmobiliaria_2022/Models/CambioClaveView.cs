using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_2022.Models
{
    public class CambioClaveView
    {
        [Display(Name = "Clave Vieja")]
        [DataType(DataType.Password)]
        public string ClaveVieja { get; set; }

        [Display(Name = "Clave Nueva")]
        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(50, ErrorMessage = "La clave debe tener entre 3 y 50 caracteres", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public String ClaveNueva { get; set; }

        [Display(Name = "Repetir Clave")]
        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(50, ErrorMessage = "La clave debe tener entre 3 y 50 caracteres", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Compare("ClaveNueva")]
        public String ClaveRepeticion { get; set; }


    }
}
