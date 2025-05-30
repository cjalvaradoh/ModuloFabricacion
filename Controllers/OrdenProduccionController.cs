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

        public async Task<IActionResult> Index()
        {
            var ordenes = await _context.OrdenProduccion
                .Include(o => o.Producto)
                .Include(o => o.Materiales).ThenInclude(m => m.Material)
                .ToListAsync();

            return View(ordenes);
        }

        public async Task<IActionResult> Details(int id)
        {
            var orden = await _context.OrdenProduccion
                .Include(o => o.Producto)
                .Include(o => o.Materiales).ThenInclude(m => m.Material)
                .FirstOrDefaultAsync(o => o.Ordenid == id);

            if (orden == null) return NotFound();

            return PartialView("_OrdenProduccionDetailsPartial", orden);
        }

        public IActionResult Create()
        {
            ViewData["Productoid"] = new SelectList(_context.Producto, "ProductoId", "Codigo");

            var ultimoId = _context.OrdenProduccion.OrderByDescending(o => o.Ordenid).Select(o => o.Ordenid).FirstOrDefault();
            ViewBag.CodigoGenerado = $"PR{(ultimoId + 1):D3}";

            ViewBag.MaterialesDisponibles = _context.Material
                .Select(m => new SelectListItem { Value = m.MaterialId.ToString(), Text = m.Nombre })
                .ToList();

            return PartialView("_OrdenProduccionCreatePartial", new OrdenProduccion());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Productoid,Cantidad,Estado,FechaInicio,FechaEntrega")] OrdenProduccion ordenProduccion, int[] MaterialIds)
        {
            // Validación para evitar valores por defecto en fechas
            if (ordenProduccion.FechaInicio == default || ordenProduccion.FechaEntrega == default)
            {
                ModelState.AddModelError("", "Debes ingresar fechas válidas.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(ordenProduccion);
                await _context.SaveChangesAsync();

                foreach (var materialId in MaterialIds)
                {
                    _context.OrdenMateriales.Add(new OrdenMaterial
                    {
                        Ordenid = ordenProduccion.Ordenid,
                        Materialid = materialId,
                        CantidadUtilizada = 1,
                        Desperdicio = 0
                    });
                }

                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Orden creada correctamente";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Productoid"] = new SelectList(_context.Producto, "ProductoId", "Codigo", ordenProduccion.Productoid);

            ViewBag.MaterialesDisponibles = _context.Material
                .Select(m => new SelectListItem
                {
                    Value = m.MaterialId.ToString(),
                    Text = m.Nombre
                }).ToList();

            return PartialView("_OrdenProduccionCreatePartial", ordenProduccion);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var orden = await _context.OrdenProduccion
                .Include(o => o.Producto)
                .Include(o => o.Materiales).ThenInclude(m => m.Material)
                .FirstOrDefaultAsync(o => o.Ordenid == id);

            if (orden == null) return NotFound();

            return PartialView("_OrdenProduccionEditPartial", orden);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Ordenid,Productoid,Cantidad,Estado,FechaInicio,FechaEntrega")] OrdenProduccion ordenProduccion)
        {
            if (id != ordenProduccion.Ordenid) return NotFound();

            if (!ModelState.IsValid)
            {
                ordenProduccion.Producto = await _context.Producto.FirstOrDefaultAsync(p => p.ProductoId == ordenProduccion.Productoid);
                ordenProduccion.Materiales = await _context.OrdenMaterial
                    .Where(om => om.Ordenid == ordenProduccion.Ordenid)
                    .Include(om => om.Material)
                    .ToListAsync();

                return PartialView("_OrdenProduccionEditPartial", ordenProduccion);
            }

            try
            {
                _context.Update(ordenProduccion);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdenProduccionExists(ordenProduccion.Ordenid)) return NotFound();
                throw;
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ordenProduccion = await _context.OrdenProduccion
                .Include(o => o.Producto)
                .Include(o => o.Materiales).ThenInclude(om => om.Material)
                .FirstOrDefaultAsync(m => m.Ordenid == id);

            if (ordenProduccion == null) return NotFound();

            return View(ordenProduccion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var orden = await _context.OrdenProduccion
                .Include(o => o.Materiales)
                .Include(o => o.Seguimientos)
                .Include(o => o.Reportes)
                .FirstOrDefaultAsync(m => m.Ordenid == id);

            if (orden == null) return NotFound();

            if (orden.Reportes?.Any() == true) _context.Reporte.RemoveRange(orden.Reportes);
            if (orden.Materiales?.Any() == true) _context.OrdenMaterial.RemoveRange(orden.Materiales);
            if (orden.Seguimientos?.Any() == true) _context.SeguimientoProduccion.RemoveRange(orden.Seguimientos);

            _context.OrdenProduccion.Remove(orden);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Orden eliminada correctamente";
            return RedirectToAction(nameof(Index));

        }

        private bool OrdenProduccionExists(int id)
        {
            return _context.OrdenProduccion.Any(e => e.Ordenid == id);
        }
    }
}
