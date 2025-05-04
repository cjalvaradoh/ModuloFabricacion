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
    public class ReportesController : Controller
    {
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Reportes
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Reporte_1.Include(r => r.Orden).Include(r => r.Producto);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Reportes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reporte = await _context.Reporte_1
                .Include(r => r.Orden)
                .Include(r => r.Producto)
                .FirstOrDefaultAsync(m => m.ReporteId == id);
            if (reporte == null)
            {
                return NotFound();
            }

            return View(reporte);
        }

        // GET: Reportes/Create
        public IActionResult Create()
        {

            ViewData["OrdenId"] = new SelectList(_context.Set<OrdenProduccion>(), "Ordenid", "Estado");
            ViewData["ProductoId"] = new SelectList(_context.Producto, "ProductoId", "Codigo");
            return View();
        }

        // POST: Reportes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReporteId,OrdenId,ProductoId,Cantidad,TiempoProduccion,Estado,FechaGeneracion")] Reporte reporte)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reporte);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrdenId"] = new SelectList(_context.Set<OrdenProduccion>(), "Ordenid", "Estado", reporte.OrdenId);
            ViewData["ProductoId"] = new SelectList(_context.Producto, "ProductoId", "Codigo", reporte.ProductoId);
            return View(reporte);
        }

        // GET: Reportes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reporte = await _context.Reporte_1.FindAsync(id);
            if (reporte == null)
            {
                return NotFound();
            }
            ViewData["OrdenId"] = new SelectList(_context.Set<OrdenProduccion>(), "Ordenid", "Estado", reporte.OrdenId);
            ViewData["ProductoId"] = new SelectList(_context.Producto, "ProductoId", "Codigo", reporte.ProductoId);
            return View(reporte);
        }

        // POST: Reportes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("ReporteId,OrdenId,ProductoId,Cantidad,TiempoProduccion,Estado")] Reporte reporte)
        {
            if (reporte.Cantidad <= 0)
            {
                TempData["ErrorMessage"] = "La cantidad debe ser mayor que 0";
                return RedirectToAction(nameof(Index));
            }

            var original = await _context.Reporte_1.FindAsync(reporte.ReporteId);
            if (original == null)
            {
                return NotFound();
            }

            original.OrdenId = reporte.OrdenId;
            original.ProductoId = reporte.ProductoId;
            original.Cantidad = reporte.Cantidad;
            original.TiempoProduccion = reporte.TiempoProduccion;
            original.Estado = reporte.Estado;

            try
            {
                _context.Update(original);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Reporte actualizado correctamente";
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "Error al guardar: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Reportes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reporte = await _context.Reporte_1
                .Include(r => r.Orden)
                .Include(r => r.Producto)
                .FirstOrDefaultAsync(m => m.ReporteId == id);
            if (reporte == null)
            {
                return NotFound();
            }

            return View(reporte);
        }

        // POST: Reportes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reporte = await _context.Reporte_1.FindAsync(id);
            if (reporte != null)
            {
                _context.Reporte_1.Remove(reporte);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReporteExists(int id)
        {
            return _context.Reporte_1.Any(e => e.ReporteId == id);
        }
    }
}
