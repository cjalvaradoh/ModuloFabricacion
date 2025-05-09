using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using caobaModeloFabricacion.Data;
using caobaModeloFabricacion.Models;

namespace caobaModeloFabricacion.Controllers
{
    public class SeguimientoProduccionsController : Controller
    {
        private readonly AppDbContext _context;

        public SeguimientoProduccionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: SeguimientoProduccions
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.SeguimientoProduccion.Include(s => s.Operario).Include(s => s.OrdenProduccion);
            return View(await appDbContext.ToListAsync());
        }

        // GET: SeguimientoProduccions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seguimientoProduccion = await _context.SeguimientoProduccion
                .Include(s => s.Operario)
                .Include(s => s.OrdenProduccion)
                .FirstOrDefaultAsync(m => m.SeguimientoId == id);
            if (seguimientoProduccion == null)
            {
                return NotFound();
            }

            return View(seguimientoProduccion);
        }

        // GET: SeguimientoProduccions/Create
        public IActionResult Create()
        {
            ViewData["OperarioId"] = new SelectList(_context.Operario, "Id", "Id");
            ViewData["OrdenProduccionId"] = new SelectList(_context.OrdenProduccion, "Ordenid", "Estado");
            return View();
        }

        // POST: SeguimientoProduccions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SeguimientoId,OrdenProduccionId,OperarioId,Estado,Avance,TiempoTrabajado,MaterialConsumido,FechaActualizacion,Comentarios")] SeguimientoProduccion seguimientoProduccion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(seguimientoProduccion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OperarioId"] = new SelectList(_context.Operario, "Id", "Id", seguimientoProduccion.OperarioId);
            ViewData["OrdenProduccionId"] = new SelectList(_context.OrdenProduccion, "Ordenid", "Estado", seguimientoProduccion.OrdenProduccionId);
            return View(seguimientoProduccion);
        }

        // GET: SeguimientoProduccions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seguimientoProduccion = await _context.SeguimientoProduccion.FindAsync(id);
            if (seguimientoProduccion == null)
            {
                return NotFound();
            }
            ViewData["OperarioId"] = new SelectList(_context.Operario, "Id", "Id", seguimientoProduccion.OperarioId);
            ViewData["OrdenProduccionId"] = new SelectList(_context.OrdenProduccion, "Ordenid", "Estado", seguimientoProduccion.OrdenProduccionId);
            return View(seguimientoProduccion);
        }

        // POST: SeguimientoProduccions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SeguimientoId,OrdenProduccionId,OperarioId,Estado,Avance,TiempoTrabajado,MaterialConsumido,FechaActualizacion,Comentarios")] SeguimientoProduccion seguimientoProduccion)
        {
            if (id != seguimientoProduccion.SeguimientoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seguimientoProduccion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeguimientoProduccionExists(seguimientoProduccion.SeguimientoId))
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
            ViewData["OperarioId"] = new SelectList(_context.Operario, "Id", "Id", seguimientoProduccion.OperarioId);
            ViewData["OrdenProduccionId"] = new SelectList(_context.OrdenProduccion, "Ordenid", "Estado", seguimientoProduccion.OrdenProduccionId);
            return View(seguimientoProduccion);
        }

        // GET: SeguimientoProduccions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seguimientoProduccion = await _context.SeguimientoProduccion
                .Include(s => s.Operario)
                .Include(s => s.OrdenProduccion)
                .FirstOrDefaultAsync(m => m.SeguimientoId == id);
            if (seguimientoProduccion == null)
            {
                return NotFound();
            }

            return View(seguimientoProduccion);
        }

        // POST: SeguimientoProduccions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seguimientoProduccion = await _context.SeguimientoProduccion.FindAsync(id);
            if (seguimientoProduccion != null)
            {
                _context.SeguimientoProduccion.Remove(seguimientoProduccion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SeguimientoProduccionExists(int id)
        {
            return _context.SeguimientoProduccion.Any(e => e.SeguimientoId == id);
        }
    }
}
