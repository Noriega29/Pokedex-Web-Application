using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pokedex.Models;
using Pokedex.Models.ViewModels;
using System.Diagnostics;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Pokedex.Controllers
{
    public class PokedexController : Controller
    {
        private readonly PokedexContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PokedexController(PokedexContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        //GET: Pokedex
        public async Task<IActionResult> Index()
        {
            var pokemons = await _context.Pokemons.OrderBy(p => p.N)
                .Include(m => m.PokemonsTipos)
                .ToListAsync();

            var count = await _context.Tipos.CountAsync();

            for (int i = 1; i <= count; i++)
            {
                var index = await _context.Tipos
                    .Where(x => x.IdTipo == i)
                    .Select(y => y.IdTipo)
                    .FirstOrDefaultAsync();

                var tipo = await _context.Tipos
                    .Where(x => x.IdTipo == i)
                    .Select(y => y.Nombre)
                    .FirstOrDefaultAsync();

                ViewData[$"{index}"] = tipo;
            }
            
            return View(pokemons);
        }

        //GET: Pokedex/Details/5
        public async Task<IActionResult> Details(string? pokemon)
        {
            if (pokemon == null)
            {
                return NotFound();
            }

            var poke = await _context.Pokemons
                .FirstOrDefaultAsync(m => m.Nombre == pokemon);

            if (poke == null)
            {
                return NotFound();
            }
            return View(poke);
        }
        // Método Buscar que recibe el parámetro nombre y que devuelve una vista parcial con los resultados
        // que coincidan con el nombre.
        public async Task<IActionResult> GetSuggestions(string valor)
        {
            // Realizar la consulta a la base de datos
            var pokemons = await _context.Pokemons
                .Where(p => p.Nombre.Contains(valor))
                .OrderBy(n => n.Nombre)
                .ToListAsync();

            // Devolver una vista parcial con los pokemons
            return PartialView("_PartialViewSuggestions", pokemons);
        }

        // GET: Pokedex/Create
        public IActionResult Create()
        {
            ViewData["Tipos"] = new SelectList(_context.Tipos.OrderBy(m => m.Nombre), "IdTipo", "Nombre");

            return View();
        }

        //POST: Pokedex/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PokemonViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var funtion = new Funciones();
                var ruta = funtion.GetImage(_webHostEnvironment.WebRootPath,
                    "images\\48x48\\",
                    viewModel.Nombre,
                    viewModel.FormFile.ContentType);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await viewModel.FormFile.CopyToAsync(stream);
                }


                var pokemon = new Pokemon
                {
                    N = viewModel.N,
                    Nombre = viewModel.Nombre,
                    Descripcion = viewModel.Descripcion,
                    Icono = Path.GetFileName($"{viewModel.Nombre}.{funtion.GetFormato(viewModel.FormFile.ContentType, '/')}")
                };

                await CreatePokemon(pokemon, viewModel.PrimerTipo, viewModel.SegundoTipo);
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        //GET: Pokedex/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["Tipos"] = new SelectList(_context.Tipos.OrderBy(m => m.Nombre), "IdTipo", "Nombre");

            var pokemon = await _context.Pokemons.FindAsync(id);
            
            var viewModel = new PokemonViewModel
            {
                Id = pokemon.IdPokemon,
                N = pokemon.N,
                Nombre = pokemon.Nombre,
                PrimerTipo = await _context.PokemonsTipos
                    .OrderBy(o => o.Id)
                    .Where(n => n.IdPokemon == id)
                    .Select(m => m.IdTipo)
                    .FirstAsync(),
                SegundoTipo = await _context.PokemonsTipos
                    .OrderBy(o => o.Id)
                    .Where(n => n.IdPokemon == id)
                    .Select(m => m.IdTipo)
                    .LastAsync(),
                Descripcion = pokemon.Descripcion,
                Icono = pokemon.Icono,
            };

            if (pokemon == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        //POST: Pokedex/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PokemonViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var funtion = new Funciones();
                    var ruta = funtion.GetImage(_webHostEnvironment.WebRootPath,
                        "images\\48x48\\",
                        viewModel.Nombre,
                        viewModel.FormFile.ContentType);

                    using (var stream = new FileStream(ruta, FileMode.Create))
                    {
                        await viewModel.FormFile.CopyToAsync(stream);
                    }


                    var pokemon = new Pokemon
                    {
                        IdPokemon = id,
                        N = viewModel.N,
                        Nombre = viewModel.Nombre,
                        Descripcion = viewModel.Descripcion,
                        Icono = Path.GetFileName($"{viewModel.Nombre}.{funtion.GetFormato(viewModel.FormFile.ContentType, '/')}")
                    };

                    await UpdatePokemon(pokemon, viewModel.PrimerTipo, viewModel.SegundoTipo);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PokemonExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        //GET: Pokedex/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pokemon = await _context.Pokemons
                .FirstOrDefaultAsync(m => m.IdPokemon == id);
            if (pokemon == null)
            {
                return NotFound();
            }

            return View(pokemon);
        }

        //POST: Pokedex/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pokemon = await _context.Pokemons.FindAsync(id);
            if (pokemon != null)
            {
                _context.Pokemons.Remove(pokemon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PokemonExists(int id)
        {
            return _context.Pokemons.Any(e => e.IdPokemon == id);
        }


        /// <summary>
        /// Crea un pokémon a la DB.
        /// </summary>
        /// <param name="pokemon">Pokémon a crear.</param>
        /// <param name="tipo1">Primer tipo del pokémon.</param>
        /// <param name="tipo2">Segundo tipo del pokémon.</param>
        private async Task CreatePokemon(Pokemon pokemon, int tipo1, int tipo2)
        {

            var a = new PokemonsTipo
            {
                IdPokemonNavigation = pokemon,
                IdTipo = tipo1
            };

            var b = new PokemonsTipo
            {
                IdPokemonNavigation = pokemon,
                IdTipo = tipo2
            };

            if (a.IdTipo == 0)
            {
                _context.AddRange(pokemon, a);
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.AddRange(pokemon, a, b);
                await _context.SaveChangesAsync();
            }
        }


        /// <summary>
        /// Actualiza un pokémon a la DB.
        /// </summary>
        /// <param name="pokemon">Pokémon a actualizar.</param>
        /// <param name="tipo1">Primer tipo del pokémon.</param>
        /// <param name="tipo2">Segundo tipo del pokémon.</param>
        private async Task UpdatePokemon(Pokemon pokemon, int tipo1, int tipo2)
        {
            //Cuenta cuantos tipos tiene el pokémon
            var count = _context.PokemonsTipos
                .OrderBy(i => i.Id)
                .Where(p => p.IdPokemon == pokemon.IdPokemon)
                .Count();

            //El pokémon tiene dos tipos.
            if (count == 2)
            {
                //El segundo tipo es diferente del primero y de cero.
                if (tipo2 != tipo1 && tipo2 != 0)
                {
                    //Id del primer tipo previo a actualizar.
                    var idTemp1 = _context.PokemonsTipos
                        .OrderBy(i => i.Id)
                        .Where(p => p.IdPokemon == pokemon.IdPokemon)
                        .Select(i => i.Id)
                        .First();
                    //Id del segundo tipo previo a actualizar.
                    var idTemp2 = _context.PokemonsTipos
                        .OrderBy(i => i.Id)
                        .Where(p => p.IdPokemon == pokemon.IdPokemon)
                        .Select(i => i.Id)
                        .Last();
                    //Asigna el primer tipo nuevo
                    var a = new PokemonsTipo
                    {
                        Id = idTemp1,
                        IdPokemonNavigation = pokemon,
                        IdTipo = tipo1
                    };
                    //Asigna el segundo tipo nuevo
                    var b = new PokemonsTipo
                    {
                        Id = idTemp2,
                        IdPokemonNavigation = pokemon,
                        IdTipo = tipo2
                    };

                    _context.UpdateRange(pokemon, a, b);
                    await _context.SaveChangesAsync();
                }
                //El segundo tipo es igual al primero o a cero.
                else
                {
                    //Id del primer tipo previo a actualizar.
                    var idTemp1 = _context.PokemonsTipos
                        .OrderBy(i => i.Id)
                        .Where(p => p.IdPokemon == pokemon.IdPokemon)
                        .Select(i => i.Id)
                        .First();
                    //Id del segundo tipo previo a actualizar.
                    var idTemp2 = _context.PokemonsTipos
                        .OrderBy(i => i.Id)
                        .Where(p => p.IdPokemon == pokemon.IdPokemon)
                        .Select(i => i.Id)
                        .Last();
                    //Asigna el primer tipo nuevo

                    var a = new PokemonsTipo
                    {
                        Id = idTemp1,
                        IdPokemonNavigation = pokemon,
                        IdTipo = tipo1
                    };

                    var b = new PokemonsTipo
                    {
                        Id = idTemp2
                    };
                    
                    _context.UpdateRange(pokemon, a);
                    _context.Remove(b);
                    await _context.SaveChangesAsync();
                    
                }
            }
            //El pokémon tiene un tipo.
            else
            {
                //El segundo tipo es diferente del primero y de cero.
                if (tipo2 != tipo1 && tipo2 != 0)
                {
                    //Id del primer tipo previo a actualizar.
                    var idTemp1 = _context.PokemonsTipos
                        .OrderBy(i => i.Id)
                        .Where(p => p.IdPokemon == pokemon.IdPokemon)
                        .Select(i => i.Id)
                        .First();
                    //Asigna el primer tipo nuevo
                    var a = new PokemonsTipo
                    {
                        Id = idTemp1,
                        IdPokemonNavigation = pokemon,
                        IdTipo = tipo1
                    };
                    //Crea el segundo tipo
                    var b = new PokemonsTipo
                    {
                        IdPokemonNavigation = pokemon,
                        IdTipo = tipo2
                    };

                    _context.UpdateRange(pokemon, a);
                    _context.Add(b);
                    await _context.SaveChangesAsync();
                }
                //El segundo tipo es igual al primero o a cero.
                else
                {
                    //Id del primer tipo previo a actualizar.
                    var idTemp1 = _context.PokemonsTipos
                        .OrderBy(i => i.Id)
                        .Where(p => p.IdPokemon == pokemon.IdPokemon)
                        .Select(i => i.Id)
                        .First();
                    //Asigna el primer tipo nuevo.
                    var a = new PokemonsTipo
                    {
                        Id = idTemp1,
                        IdPokemonNavigation = pokemon,
                        IdTipo = tipo1
                    };

                    _context.UpdateRange(pokemon, a);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
