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
    public class OrdenProduccionsController : Controller
    {
        private readonly AppDbContext _context;

        public OrdenProduccionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: OrdenProduccions
        public async Task<IActionResult> Index(string searchString, string estado)
        {
            var ordenes = from o in _context.OrdenesProduccion.Include(o => o.Producto)
                          select o;

            if (!string.IsNullOrEmpty(searchString))
            {
                ordenes = ordenes.Where(o => o.Producto.Codigo.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(estado))
            {
                ordenes = ordenes.Where(o => o.Estado == estado);
            }

            // Para llenar el dropdown de estados en la vista
            ViewBag.Estados = new SelectList(new List<string> { "Pendiente", "En proceso", "Finalizada", "Cancelada" });

            return View(await ordenes.ToListAsync());
        }

        // GET: OrdenProduccions/Create
        public IActionResult Create()
        {
            ViewData["ProductoId"] = new SelectList(_context.Producto, "ProductoId", "Codigo");
            return View();
        }

        // POST: OrdenProduccions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductoId,Cantidad,Estado,FechaInicio,FechaEntrega")] OrdenProduccion ordenProduccion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ordenProduccion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductoId"] = new SelectList(_context.Producto, "ProductoId", "Codigo", ordenProduccion.ProductoId);
            return View(ordenProduccion);
        }

        // GET: OrdenProduccions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenProduccion = await _context.OrdenesProduccion.FindAsync(id);
            if (ordenProduccion == null)
            {
                return NotFound();
            }
            ViewData["ProductoId"] = new SelectList(_context.Producto, "ProductoId", "Codigo", ordenProduccion.ProductoId);
            return View(ordenProduccion);
        }

        // POST: OrdenProduccions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductoId,Cantidad,Estado,FechaInicio,FechaEntrega")] OrdenProduccion ordenProduccion)
        {
            if (id != ordenProduccion.Id)
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
                    if (!OrdenProduccionExists(ordenProduccion.Id))
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
            ViewData["ProductoId"] = new SelectList(_context.Producto, "ProductoId", "Codigo", ordenProduccion.ProductoId);
            return View(ordenProduccion);
        }

        // GET: OrdenProduccions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenProduccion = await _context.OrdenesProduccion
                .Include(o => o.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordenProduccion == null)
            {
                return NotFound();
            }

            return View(ordenProduccion);
        }

        // POST: OrdenProduccions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordenProduccion = await _context.OrdenesProduccion.FindAsync(id);
            if (ordenProduccion != null)
            {
                _context.OrdenesProduccion.Remove(ordenProduccion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdenProduccionExists(int id)
        {
            return _context.OrdenesProduccion.Any(e => e.Id == id);
        }
    }
}
