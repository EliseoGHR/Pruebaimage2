using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Pruebaimage2.Models
{
    public class Carro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CarroId { get; set; }

        [Required(ErrorMessage = "El modelo del carro es obligatorio.")]
        public string Modelo { get; set; }

        // Propiedad de descripción del carro, nullable
        public string? Descripcion { get; set; }
        [Required(ErrorMessage = "El precio del carro es obligatorio.")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Precio { get; set; }
        // Propiedad de la imagen del carro
        public byte[]? Imagen { get; set; }

        // Propiedad de navegación para la relación con Marca
        public int? MarcaId { get; set; }
        [ForeignKey("MarcaId")]
        public virtual Marca? Marca { get; set; }

    }
}
