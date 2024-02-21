using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Pruebaimage2.Models
{
    public class Marca
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MarcaId { get; set; }


        [Required(ErrorMessage = "El nombre de la marca es obligatorio.")]
        public string Nombre { get; set; }

        // Relación uno a muchos con Carro
        public virtual ICollection<Carro> Carros { get; set; }
    }
}
