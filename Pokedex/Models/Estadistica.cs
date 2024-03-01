using System;
using System.Collections.Generic;

namespace Pokedex.Models;

public partial class Estadistica
{
    public int IdEstadistica { get; set; }

    public int Ps { get; set; }

    public int Ataque { get; set; }

    public int Defensa { get; set; }

    public int AtaqueEsp { get; set; }

    public int DefensaEsp { get; set; }

    public int Velocidad { get; set; }

    public int IdPokemon { get; set; }

    public virtual Pokemon IdPokemonNavigation { get; set; } = null!;
}
