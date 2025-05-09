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
    public class OrdenProduccionController : Controller
    {
        private readonly AppDbContext _context;

        public OrdenProduccionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: OrdenProduccion
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.OrdenProduccion.Include(o => o.Producto);
            return View(await appDbContext.ToListAsync());
        }

        // GET: OrdenProduccion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenProduccion = await _context.OrdenProduccion
                .Include(o => o.Producto)
                .FirstOrDefaultAsync(m => m.Ordenid == id);
            if (ordenProduccion == null)
            {
                return NotFound();
            }

            return View(ordenProduccion);
        }

        // GET: OrdenProduccion/Create
        public IActionResult Create()
        {
            ViewData["Productoid"] = new SelectList(_context.Producto, "ProductoId", "Codigo");
            return View();
        }

        // POST: OrdenProduccion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ordenid,Productoid,Cantidad,Estado,FechaInicio,FechaEntrega")] OrdenProduccion ordenProduccion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ordenProduccion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Productoid"] = new SelectList(_context.Producto, "ProductoId", "Codigo", ordenProduccion.Productoid);
            return View(ordenProduccion);
        }

        // GET: OrdenProduccion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenProduccion = await _context.OrdenProduccion.FindAsync(id);
            if (ordenProduccion == null)
            {
                return NotFound();
            }
            ViewData["Productoid"] = new SelectList(_context.Producto, "ProductoId", "Codigo", ordenProduccion.Productoid);
            return View(ordenProduccion);
        }

        // POST: OrdenProduccion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Ordenid,Productoid,Cantidad,Estado,FechaInicio,FechaEntrega")] OrdenProduccion ordenProduccion)
        {
            if (id != ordenProduccion.Ordenid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordenProduccion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdenProduccionExists(ordenProduccion.Ordenid))
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
            ViewData["Productoid"] = new SelectList(_context.Producto, "ProductoId", "Codigo", ordenProduccion.Productoid);
            return View(ordenProduccion);
        }

        // GET: OrdenProduccion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenProduccion = await _context.OrdenProduccion
                .Include(o => o.Producto)
                .FirstOrDefaultAsync(m => m.Ordenid == id);
            if (ordenProduccion == null)
            {
                return NotFound();
            }

            return View(ordenProduccion);
        }

        // POST: OrdenProduccion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordenProduccion = await _context.OrdenProduccion.FindAsync(id);
            if (ordenProduccion != null)
            {
                _context.OrdenProduccion.Remove(ordenProduccion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdenProduccionExists(int id)
        {
            return _context.OrdenProduccion.Any(e => e.Ordenid == id);
        }
    }
}
