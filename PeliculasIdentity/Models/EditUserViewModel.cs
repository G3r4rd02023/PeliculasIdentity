using System.ComponentModel.DataAnnotations;

namespace PeliculasIdentity.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; } = null!;

        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Nombre { get; set; } = null!;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}