using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using caobaModeloFabricacion.Data;
using caobaModeloFabricacion.Models;
using caobaModeloFabricacion.ViewModels;

namespace caobaModeloFabricacion.Controllers
{
    public class SeguimientoProduccionsController : Controller
    {
        private readonly AppDbContext _context;

        public SeguimientoProduccionsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _context.SeguimientoProduccion
                .Include(s => s.Departamento)
                .Include(s => s.OrdenProduccion)
                .ToListAsync();

            return View(data);
        }

        public IActionResult CreatePartial()
        {
            // Obtener los departamentos con nombres compatibles para el SelectList
            var departamentos = _context.Departamento
                .Select(d => new { Id = d.DepartamentoId, Nombre = d.Nombre })
                .ToList();

            // Llenar combo de departamentos
            ViewData["DepartamentoList"] = new SelectList(departamentos, "Id", "Nombre");

            // Combo vacío de operarios (se llenará por JS al seleccionar departamento)
            ViewData["OperarioList"] = new SelectList(Enumerable.Empty<SelectListItem>());

            // Cargar órdenes
            ViewData["OrdenProduccionId"] = new SelectList(
                _context.OrdenProduccion,
                "Ordenid",
                "Ordenid"
            );

            return PartialView("CreatePartial", new SeguimientoProduccionViewModel());
        }







        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SeguimientoProduccion model)
        {
            // Validaciones manuales (sin anotaciones en el modelo)
            if (model.OrdenProduccionId == 0)
                ModelState.AddModelError("OrdenProduccionId", "Seleccione una orden.");

            if (model.OperarioId == 0)
                ModelState.AddModelError("OperarioId", "Seleccione un operario.");

            if (model.Avance == null || model.Avance <= 0)
                ModelState.AddModelError("Avance", "Ingrese un avance válido.");

            if (model.TiempoTrabajado == null || model.TiempoTrabajado <= 0)
                ModelState.AddModelError("TiempoTrabajado", "Ingrese el tiempo trabajado.");

            if (model.MaterialConsumido == null || model.MaterialConsumido <= 0)
                ModelState.AddModelError("MaterialConsumido", "Ingrese el material consumido.");

            if (string.IsNullOrEmpty(model.Estado))
                ModelState.AddModelError("Estado", "Seleccione un estado.");

            // Buscar operario para asignar departamento
            var operario = await _context.Operario.FirstOrDefaultAsync(o => o.Id == model.OperarioId);
            if (operario == null)
            {
                ModelState.AddModelError("OperarioId", "El operario seleccionado no existe.");
            }
            else
            {
                model.DepartamentoId = operario.DepartamentoId ?? 0;
                if (model.DepartamentoId == 0)
                    ModelState.AddModelError("OperarioId", "El operario no tiene departamento asignado.");
            }

            // Si hay errores, regresar la vista con combos recargados
            if (!ModelState.IsValid)
            {
                ViewData["OrdenProduccionId"] = new SelectList(_context.OrdenProduccion, "Ordenid", "Ordenid");

                ViewData["DepartamentoList"] = new SelectList(_context.Departamento, "DepartamentoId", "Nombre");

                Response.StatusCode = 400;

                var viewModel = new SeguimientoProduccionViewModel
                {
                    OrdenProduccionId = model.OrdenProduccionId,
                    OperarioId = model.OperarioId,
                    Avance = model.Avance,
                    TiempoTrabajado = model.TiempoTrabajado,
                    MaterialConsumido = model.MaterialConsumido,
                    Estado = model.Estado,
                    Comentarios = model.Comentarios,
                    DepartamentoId = model.DepartamentoId
                };

                return PartialView("CreatePartial", viewModel);
            }



            model.FechaActualizacion = DateTime.Now;
            _context.Add(model);
            await _context.SaveChangesAsync();

            return Content("OK");
        }




        public async Task<IActionResult> GetOperariosByOrden(int ordenId)
        {
            var orden = await _context.OrdenProduccion.FirstOrDefaultAsync(o => o.Ordenid == ordenId);
            if (orden == null) return NotFound();

            var operarios = await _context.Operario
                .Where(o => o.DepartamentoId == orden.DepartamentoId)
                .Select(o => new { id = o.Id, nombre = o.Carnet + " - " + o.Nombre })
                .ToListAsync();

            return Json(operarios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SeguimientoProduccion model)
        {
            if (!ModelState.IsValid)
            {
                var operario = await _context.Operario
                    .Where(o => o.DepartamentoId == model.DepartamentoId && o.Estado == "Activo")
                    .FirstOrDefaultAsync();

                ViewBag.NombreOperario = operario != null ? $"{operario.CodigoOperario} - {operario.Nombre}" : "No asignado";
                ViewBag.OrdenProduccionId = new SelectList(_context.OrdenProduccion, "Ordenid", "Ordenid", model.OrdenProduccionId);

                return PartialView("_EditSeguimiento", model);
            }

            var entity = await _context.SeguimientoProduccion.FindAsync(model.SeguimientoId);
            if (entity == null) return NotFound();

            entity.Estado = model.Estado;
            entity.Avance = model.Avance;
            entity.MaterialConsumido = model.MaterialConsumido;
            entity.TiempoTrabajado = model.TiempoTrabajado;
            entity.Comentarios = model.Comentarios;
            entity.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return Content("OK");
        }




        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var entity = await _context.SeguimientoProduccion
                .Include(s => s.OrdenProduccion)
                .FirstOrDefaultAsync(s => s.SeguimientoId == id);

            if (entity == null) return NotFound();

            // Obtener operario activo del departamento
            var operario = await _context.Operario
                .Where(o => o.DepartamentoId == entity.DepartamentoId && o.Estado == "Activo")
                .FirstOrDefaultAsync();

            entity.OperarioId = operario?.Id ?? 0;
            ViewBag.NombreOperario = operario != null ? $"{operario.CodigoOperario} - {operario.Nombre}" : "No asignado";

            ViewBag.OrdenProduccionId = new SelectList(_context.OrdenProduccion, "Ordenid", "Ordenid", entity.OrdenProduccionId);

            return PartialView("_EditSeguimiento", entity);
        }






        public async Task<IActionResult> DetailsPartial(int id)
        {
            var entity = await _context.SeguimientoProduccion
                .Include(s => s.Departamento)
                .Include(s => s.OrdenProduccion)
                .FirstOrDefaultAsync(s => s.SeguimientoId == id);

            if (entity == null) return NotFound();

            // Buscar el operario que pertenece al mismo departamento
            var operario = await _context.Operario
                .Where(o => o.DepartamentoId == entity.DepartamentoId)
                .OrderBy(o => o.Id) // si hay varios, tomar el primero 
                .FirstOrDefaultAsync();

            if (operario != null)
            {
                entity.OperarioNombre = $"{operario.CodigoOperario} - {operario.Nombre}";
            }
            else
            {
                entity.OperarioNombre = "No asignado";
            }

            return PartialView("_DetailsSeguimiento", entity);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.SeguimientoProduccion
                .Include(s => s.Departamento)
                .Include(s => s.OrdenProduccion)
                .FirstOrDefaultAsync(s => s.SeguimientoId == id);

            if (entity == null) return NotFound();

            return PartialView("_DeleteSeguimiento", entity);
        }

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

        public async Task<IActionResult> TablePartial()
        {
            var data = await _context.SeguimientoProduccion
                .Include(s => s.Departamento)
                .Include(s => s.OrdenProduccion)
                .ToListAsync();

            // Simular el nombre del operario basado en el departamento
            foreach (var item in data)
            {
                var operario = await _context.Operario
                    .Where(o => o.DepartamentoId == item.DepartamentoId && o.Estado == "Activo")
                    .OrderBy(o => o.Id)
                    .FirstOrDefaultAsync();

                item.OperarioNombre = operario != null
                    ? $"{operario.CodigoOperario} - {operario.Nombre}"
                    : "No asignado";
            }

            return PartialView("_SeguimientoTableBody", data);
        }
        private void CargarCombos()
        {
            ViewData["OrdenProduccionId"] = new SelectList(_context.OrdenProduccion, "Ordenid", "Ordenid");
            ViewData["OperarioList"] = new SelectList(_context.Operario
                .Where(o => o.Estado == "Activo")
                .Select(o => new { o.Id, Nombre = o.Carnet + " - " + o.Nombre })
                .ToList(), "Id", "Nombre");
        }

        [HttpGet]
        public IActionResult GetOperariosPorOrden(int ordenId)
        {
            var orden = _context.OrdenProduccion.FirstOrDefault(o => o.Ordenid == ordenId);
            if (orden == null) return Json(new { });

            var operarios = _context.Operario
                .Where(o => o.DepartamentoId == orden.DepartamentoId && o.Estado == "Activo")
                .Select(o => new SelectListItem
                {
                    Value = o.Id.ToString(),
                    Text = o.Carnet + " - " + o.Nombre
                })
                .ToList();

            return Json(operarios);
        }
        [HttpGet]
        public JsonResult GetOperariosPorDepartamento(int id)
        {
            var operarios = _context.Operario
                .Where(o => o.DepartamentoId == id && o.Estado == "Activo")
                .Select(o => new
                {
                    value = o.Id,
                    text = o.Carnet + " - " + o.Nombre
                })
                .ToList();

            return Json(operarios);
        }


    }

}
