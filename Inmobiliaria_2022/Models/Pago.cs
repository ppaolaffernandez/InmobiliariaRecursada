using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria_2022.Models
{
    public class Pago
    {
        [Key]
        public int Id { get; set; }
        public string Numero { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Importe { get; set; }
        public int ContratoId { get; set; }
        public Contrato Contrato { get; set; }
        public string Buscar { get; set; }


    }
}
