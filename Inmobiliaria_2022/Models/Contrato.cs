using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_2022.Models
{
    public class Contrato
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime FechaBaja { get; set; }
        public decimal Monto { get; set; }
        public int InmuebleId { get; set; }
        public Inmueble inmueble { get; set; }
        public int InquilinoId { get; set; }
        public Inquilino inquilino { get; set; }
        public Boolean EstaHabilitado { get; set; }

    }
}
