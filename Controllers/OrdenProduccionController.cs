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
            // Obtener el último ID registrado
            var ultimoOrdenId = _context.OrdenProduccion
                .OrderByDescending(o => o.Ordenid)
                .Select(o => o.Ordenid)
                .FirstOrDefault();

            // Asegurar que el número inicie desde 1001
            var numeroVisible = Math.Max(ultimoOrdenId + 1, 1001);

            // Enviar el código al formulario
            ViewBag.CodigoGenerado = $"PR{numeroVisible}";


            ViewBag.Productoid = new SelectList(_context.Producto, "ProductoId", "Nombre");
            ViewBag.MaterialesDisponibles = _context.Material
                .Select(m => new SelectListItem { Value = m.MaterialId.ToString(), Text = m.Nombre })
                .ToList();

            ViewBag.Departamentos = new SelectList(_context.Departamento, "DepartamentoId", "Nombre");

            return PartialView("_OrdenProduccionCreatePartial", new OrdenProduccion());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Productoid,Cantidad,Estado,FechaInicio,FechaEntrega,DepartamentoId")] OrdenProduccion ordenProduccion, int[] MaterialIds)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            // Validación 1
            if (!ModelState.IsValid || ordenProduccion.Cantidad <= 0 || MaterialIds == null || MaterialIds.Length == 0)
            {
                CargarCombos(ordenProduccion); // ✅ ¡Antes del if!
                if (isAjax)
                {
                    Response.StatusCode = 400;
                    return PartialView("_OrdenProduccionCreatePartial", ordenProduccion);
                }

                ViewData["ErrorMensaje"] = "⚠️ Complete todos los campos y seleccione al menos un material.";
                return PartialView("_OrdenProduccionCreatePartial", ordenProduccion);
            }

            // Validación 2
            if (ordenProduccion.FechaInicio == default || ordenProduccion.FechaEntrega == default || ordenProduccion.FechaEntrega < ordenProduccion.FechaInicio)
            {
                CargarCombos(ordenProduccion); // ✅ ¡Antes del if!
                if (isAjax)
                {
                    Response.StatusCode = 400;
                    return PartialView("_OrdenProduccionCreatePartial", ordenProduccion);
                }

                ViewData["ErrorMensaje"] = "⚠️ La fecha de entrega no puede ser anterior a la de inicio.";
                return PartialView("_OrdenProduccionCreatePartial", ordenProduccion);
            }

            ordenProduccion.Estado = "Pendiente";
            _context.Add(ordenProduccion);
            await _context.SaveChangesAsync();

            foreach (var materialId in MaterialIds)
            {
                var material = await _context.Material.FindAsync(materialId);
                if (material == null) continue;

                decimal cantidadUsada = ordenProduccion.Cantidad;

                if (material.Stock < cantidadUsada)
                {
                    CargarCombos(ordenProduccion); // ✅ ¡Antes del if!
                    if (isAjax)
                    {
                        Response.StatusCode = 400;
                        return PartialView("_OrdenProduccionCreatePartial", ordenProduccion);
                    }

                    ViewData["ErrorMensaje"] = $"⚠️ No hay suficiente stock del material '{material.Nombre}' (stock disponible: {material.Stock}).";
                    return PartialView("_OrdenProduccionCreatePartial", ordenProduccion);
                }

                material.Stock -= cantidadUsada;

                _context.OrdenMateriales.Add(new OrdenMaterial
                {
                    Ordenid = ordenProduccion.Ordenid,
                    Materialid = material.MaterialId,
                    CantidadUtilizada = cantidadUsada,
                    Desperdicio = 0
                });
            }

            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var ordenes = await _context.OrdenProduccion
                    .Include(o => o.Producto)
                    .Include(o => o.Materiales).ThenInclude(m => m.Material)
                    .ToListAsync();

                return PartialView("_OrdenProduccionTableBody", ordenes);
            }

            TempData["Mensaje"] = "Orden creada correctamente";
            return RedirectToAction(nameof(Index));

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
        public async Task<IActionResult> Edit(int id, [Bind("Ordenid,Productoid,Cantidad,Estado,FechaInicio,FechaEntrega,DepartamentoId")] OrdenProduccion ordenProduccion)
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

            var ordenExistente = await _context.OrdenProduccion.FindAsync(id);
            if (ordenExistente == null) return NotFound();

            ordenExistente.Estado = ordenProduccion.Estado;
            ordenExistente.FechaInicio = ordenProduccion.FechaInicio;
            ordenExistente.FechaEntrega = ordenProduccion.FechaEntrega;
            ordenExistente.DepartamentoId = ordenProduccion.DepartamentoId;

            try
            {
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

            return PartialView("_DeletePartial", ordenProduccion);
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

        private void CargarCombos(OrdenProduccion orden)
        {
            var ultimoCodigo = _context.OrdenProduccion
            .OrderByDescending(o => o.Ordenid)
            .Select(o => o.Ordenid)
            .FirstOrDefault();

            var numeroVisible = Math.Max(ultimoCodigo + 1, 1001);
            ViewBag.CodigoGenerado = $"PR{numeroVisible}";

            ViewBag.Productoid = new SelectList(_context.Producto, "ProductoId", "Nombre", orden?.Productoid);
            ViewBag.MaterialesDisponibles = _context.Material.Select(m => new SelectListItem
            {
                Value = m.MaterialId.ToString(),
                Text = m.Nombre
            }).ToList();
            ViewBag.Departamentos = new SelectList(_context.Departamento, "DepartamentoId", "Nombre", orden?.DepartamentoId);
        }

        public async Task<IActionResult> TablePartial()
        {
            var ordenes = await _context.OrdenProduccion
                .Include(o => o.Producto)
                .Include(o => o.Materiales)
                    .ThenInclude(om => om.Material)
                .ToListAsync();

            return PartialView("_OrdenProduccionTableBody", ordenes);
        }
    }
}
