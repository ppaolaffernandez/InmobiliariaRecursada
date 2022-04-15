using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_2022.Models
{
    public class LoginView
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Clave { get; set; }
    }
}
