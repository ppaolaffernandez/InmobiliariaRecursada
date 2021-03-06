using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_2022.Models
{
	public class Propietario
	{
		[Key]
		//[Display(Name = "Código")]
		public int Id { get; set; }
		[Required]
		public string Nombre { get; set; }
		[Required]
		public string Apellido { get; set; }
		[Required]
		public string Dni { get; set; }
		public string Telefono { get; set; }
		[Required, EmailAddress]
		public string Email { get; set; }
		//[Required, DataType(DataType.Password)]
		public string Clave { get; set; }
		public Boolean EstaHabilitado { get; set; }

	}
}
