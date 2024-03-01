using System;
using System.Collections.Generic;

namespace Pokedex.Models;

public partial class Pokemon
{
    public int IdPokemon { get; set; }

    public int N { get; set; }

    public string Nombre { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public string Icono { get; set; } = null!;

    public virtual ICollection<Estadistica> Estadisticas { get; set; } = new List<Estadistica>();

    public virtual ICollection<PokemonsTipo> PokemonsTipos { get; set; } = new List<PokemonsTipo>();
}
