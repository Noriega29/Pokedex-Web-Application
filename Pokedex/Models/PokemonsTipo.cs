﻿using System;
using System.Collections.Generic;

namespace Pokedex.Models;

public partial class PokemonsTipo
{
    public int Id { get; set; }

    public int IdPokemon { get; set; }

    public int IdTipo { get; set; }

    public virtual Pokemon IdPokemonNavigation { get; set; } = null!;

    public virtual Tipo IdTipoNavigation { get; set; } = null!;
}
