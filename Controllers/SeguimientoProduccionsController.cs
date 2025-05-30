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

        // INDEX
        public async Task<IActionResult> Index()
        {
            var data = await _context.SeguimientoProduccion
                .Include(s => s.Operario)
                .Include(s => s.OrdenProduccion)
                .ToListAsync();

            return View(data);
        }

        // CREATE GET
        public IActionResult CreatePartial()
        {
            ViewData["OperarioId"] = new SelectList(_context.Operario, "Id", "Carnet");
            ViewData["OrdenProduccionId"] = new SelectList(_context.OrdenProduccion, "Ordenid", "Ordenid");
            return PartialView("CreatePartial");
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePartial(SeguimientoProduccion model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["OperarioId"] = new SelectList(_context.Operario, "Id", "Carnet", model.OperarioId);
                ViewData["OrdenProduccionId"] = new SelectList(_context.OrdenProduccion, "Ordenid", "Ordenid", model.OrdenProduccionId);
                return PartialView("CreatePartial", model);
            }

            _context.Add(model);
            await _context.SaveChangesAsync();

            return Content("OK");
        }

        // EDIT GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _context.SeguimientoProduccion
                .Include(s => s.Operario)
                .Include(s => s.OrdenProduccion)
                .FirstOrDefaultAsync(s => s.SeguimientoId == id);

            if (entity == null) return NotFound();

            ViewData["OperarioId"] = new SelectList(_context.Operario, "Id", "Carnet", entity.OperarioId);
            ViewData["OrdenProduccionId"] = new SelectList(_context.OrdenProduccion, "Ordenid", "Ordenid", entity.OrdenProduccionId);

            return PartialView("_EditSeguimiento", entity);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SeguimientoProduccion model)
        {
            Console.WriteLine(">>> POST Edit recibido:");
            Console.WriteLine($"ID: {model.SeguimientoId}, Avance: {model.Avance}, Estado: {model.Estado}");

            if (!ModelState.IsValid)
            {
                ViewData["OperarioId"] = new SelectList(_context.Operario, "Id", "Carnet", model.OperarioId);
                ViewData["OrdenProduccionId"] = new SelectList(_context.OrdenProduccion, "Ordenid", "Ordenid", model.OrdenProduccionId);
                return PartialView("_EditSeguimiento", model);
            }

            try
            {
                // Validar que los IDs existan
                bool operarioExiste = await _context.Operario.AnyAsync(o => o.Id == model.OperarioId);
                bool ordenExiste = await _context.OrdenProduccion.AnyAsync(o => o.Ordenid == model.OrdenProduccionId);

                if (!operarioExiste)
                    return BadRequest("El operario seleccionado no existe.");

                if (!ordenExiste)
                    return BadRequest("La orden de producción seleccionada no existe.");

                // Buscar el registro original
                var seguimientoOriginal = await _context.SeguimientoProduccion.FindAsync(model.SeguimientoId);
                if (seguimientoOriginal == null)
                    return NotFound();

                // Actualizar campos editables
                seguimientoOriginal.OperarioId = model.OperarioId;
                seguimientoOriginal.Avance = model.Avance;
                seguimientoOriginal.MaterialConsumido = model.MaterialConsumido;
                seguimientoOriginal.TiempoTrabajado = model.TiempoTrabajado;
                seguimientoOriginal.Estado = model.Estado;
                seguimientoOriginal.Comentarios = model.Comentarios;
                seguimientoOriginal.FechaActualizacion = DateTime.Now;

                await _context.SaveChangesAsync();
                return Content("OK");
            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                Console.WriteLine(">>> ERROR INTERNO AL GUARDAR:");
                Console.WriteLine(inner);
                return StatusCode(500, "Error al guardar: " + inner);
            }
            catch (Exception ex)
            {
                Console.WriteLine(">>> ERROR GENERAL:");
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Error inesperado: " + ex.Message);
            }
        }

        // DETAILS
        public async Task<IActionResult> DetailsPartial(int id)
        {
            var entity = await _context.SeguimientoProduccion
                .Include(s => s.Operario)
                .Include(s => s.OrdenProduccion)
                .FirstOrDefaultAsync(s => s.SeguimientoId == id);

            if (entity == null) return NotFound();

            return PartialView("_DetailsSeguimiento", entity);
        }

        // DELETE GET (CONFIRMACIÓN)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.SeguimientoProduccion
                .Include(s => s.Operario)
                .Include(s => s.OrdenProduccion)
                .FirstOrDefaultAsync(s => s.SeguimientoId == id);

            if (entity == null) return NotFound();

            return PartialView("_DeleteSeguimiento", entity);
        }

        // DELETE POST (CONFIRMADO)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int SeguimientoId)
        {
            var entity = await _context.SeguimientoProduccion.FindAsync(SeguimientoId);
            if (entity == null) return NotFound();

            _context.SeguimientoProduccion.Remove(entity);
            await _context.SaveChangesAsync();

            return Content("OK");
        }
        // GET: Actualización parcial del cuerpo de la tabla
        public async Task<IActionResult> TablePartial()
        {
            var data = await _context.SeguimientoProduccion
                .Include(s => s.Operario)
                .Include(s => s.OrdenProduccion)
                .ToListAsync();

            return PartialView("_SeguimientoTableBody", data);
        }

    }
}
