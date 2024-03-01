using System;
using System.Collections.Generic;

namespace Pokedex.Models;

public partial class Tipo
{
    public int IdTipo { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<PokemonsTipo> PokemonsTipos { get; set; } = new List<PokemonsTipo>();
}
