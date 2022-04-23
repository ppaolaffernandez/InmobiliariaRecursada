using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_2022.Models
{
    public enum enRoles
    {
        Administrador = 1,
        Empleado = 2,
    }
    public class Usuario
    {
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Clave { get; set; }
        public string? Avatar { get; set; }  /*agregue esto avatar*/
        public IFormFile AvatarFile { get; set; } /*22/04/*/
        public int Rol { get; set; }
        public string RolNombre => Rol > 0 ? ((enRoles)Rol).ToString() : "";

        public static IDictionary<int, string> ObtenerRoles()
        {
            SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
            Type tipoEnumRol = typeof(enRoles);
            foreach (var valor in Enum.GetValues(tipoEnumRol))
            {
                roles.Add((int)valor, Enum.GetName(tipoEnumRol, valor));
            }
            return roles;
        }


    }
}
