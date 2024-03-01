using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pokedex.Models;

namespace Pokedex.Controllers
{
    public class TiposController : Controller
    {
        private readonly PokedexContext _context;

        public TiposController(PokedexContext context)
        {
            _context = context;
        }

        //GET: Tipos
        public async Task<IActionResult> Index()
        {
            var tipos = _context.Tipos.OrderBy(x => x.Nombre).ToListAsync();
            return View(await tipos);
        }

        //GET: Tipos/Create
        public IActionResult Create()
        {
            return View();
        }

        //POST: Tipos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tipo tipo)
        {
            if (ModelState.IsValid)
            {
                _context.Tipos.Add(tipo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipo);
        }

        //GET: Tipos/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipo = await _context.Tipos.FindAsync(id);

            if (tipo == null)
            {
                return NotFound();
            }
            return View(tipo);
        }

        //POST: Tipos/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tipo tipo)
        {
            if (id != tipo.IdTipo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoExists(tipo.IdTipo))
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
            return View(tipo);
        }

        //GET: Tipos/Delect
        public async Task<IActionResult> Delect(int? id)
        {
            if (id == null)
            {
                NotFound();
            }

            var tipo = await _context.Tipos
                .FirstOrDefaultAsync(m => m.IdTipo == id);

            if (tipo == null)
            {
                return NotFound();
            }

            return View(tipo);
        }

        //POST: Tipos/Delect
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delect(int id)
        {
            var tipo = await _context.Tipos.FindAsync(id);
            if (tipo != null)
            {
                _context.Tipos.Remove(tipo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoExists(int id)
        {
            return _context.Tipos.Any(e => e.IdTipo == id);
        }
    }
}
