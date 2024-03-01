using System.ComponentModel.DataAnnotations;


namespace Pokedex.Models.ViewModels
{
    public class PokemonViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "N.°")]
        public int N { get; set; }

        [Required]
        public string? Nombre { get; set; }

        [Required]
        [Display(Name = "Tipo #1")]
        public int PrimerTipo { get; set; }

        [Display(Name = "Tipo #2")]
        public int SegundoTipo { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        public string? Icono { get; set; }

        [Required]
        public IFormFile? FormFile { get; set; }
    }
}
