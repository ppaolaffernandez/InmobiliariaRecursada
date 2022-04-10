using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria_2022.Models
{
    public enum enTipos
    {
        Terreno = 1,
        Edificio = 2,
        Apartamento = 3,
        Casa = 4,
        Fábrica = 5,
        Local = 6,
        Mina = 7,
        Parcela = 8,
        Centro_comercial = 9,
        Finca = 10,
    }
    public class Inmueble
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Direccion { get; set; }
        [Required]
        public int Ambientes { get; set; }
        public int Tipo { get; set; }
        public string TipoNombre => Tipo > 0 ? ((enTipos)Tipo).ToString().Replace("_", " ") : "";
        public decimal Costo { get; set; }
        [Required]
        public decimal Superficie { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public int EstaPublicado { get; set; }
        public int EstaHabilitado { get; set; }
        public int PropietarioId { get; set; }
        [ForeignKey(nameof(PropietarioId))]
        public Propietario? Propietario { get; set; }//Sin ? no me deja create inmueb

        //Clave , valor (IDicictionary)
        public static IDictionary<int, string> ObtenerTiposIDictionary()
        {
            SortedDictionary<int, string> tipos = new SortedDictionary<int, string>();

            Type tipoEnumRol = typeof(enTipos);
            foreach (var valor in Enum.GetValues(tipoEnumRol))
            {
                tipos.Add((int)valor, Enum.GetName(tipoEnumRol, valor));
            }
            return tipos;



        }
    }
}
