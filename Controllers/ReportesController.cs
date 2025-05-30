using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using caobaModeloFabricacion.Data;
using caobaModeloFabricacion.Models;
using caobaModeloFabricacion.DTOs;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using caobaModeloFabricacion.DTOs.Graficas;
using caobaModeloFabricacion.DTOs.Reporte;
using System.Globalization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


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
            var reportes = await _context.Reporte.ToListAsync();

            var fechas = reportes
               .Select(r => r.FechaGeneracion.HasValue ? r.FechaGeneracion.Value.ToString("yyyy-MM-dd") : "")
                .Distinct()
                .ToList();

            var estados = reportes
                .Select(r => r.Estado)
                .Distinct()
                .ToList();

            var ordenes = reportes
                .Select(r => r.OrdenId)
                .Distinct()
                .ToList();

            var productos = await _context.Producto
                .Select(p => new SelectListItem
             {
                  Value = p.ProductoId.ToString(),
                  Text = p.Nombre 
             })
                .ToListAsync();

            ViewBag.Fechas = fechas;
            ViewBag.Estados = estados;
            ViewBag.IdOrdenes = ordenes;
            ViewBag.Productos = productos;

            return View(reportes);
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

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var data = new ReporteDataDto
            {
                OrdenesEnProceso = await _context.OrdenProduccion
                    .CountAsync(o => o.Estado == "En proceso"),
                OrdenesFinalizadas = await _context.OrdenProduccion
                    .CountAsync(o => o.Estado == "Finalizada" || o.Estado == "Completada"), // Incluye ambos posibles estados
                OrdenesPendientes = await _context.OrdenProduccion
                    .CountAsync(o => o.Estado == "Pendiente"), // Cambiado de "Pendientes" a "Pendiente"
                IndiceRetrasos = await _context.OrdenProduccion
                    .CountAsync(o => o.FechaEntrega < DateTime.Now && o.Estado != "Finalizada" && o.Estado != "Completada")
            };

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetDesempenoOrdenes()
        {
            var data = await _context.OrdenProduccion
                .Include(o => o.Producto)
                .ThenInclude(p => p.CategoriaProducto)
                .GroupBy(o => o.Producto.Nombre)
                .Select(g => new {
                    Producto = g.Key,
                    TiempoTotal = g.Select(o => _context.Reporte
                        .Where(r => r.OrdenId == o.Ordenid)
                        .Select(r => r.TiempoProduccion)
                        .FirstOrDefault()).Sum()
                })
                .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<ActionResult<List<EstadoOrdenProduccionReporteDTO>>> ObtenerReporteEstadoOrdenes()
        {
            var resultado = await _context.OrdenProduccion
                .GroupBy(o => o.Estado)
                .Select(g => new EstadoOrdenProduccionReporteDTO
                {
                    Estado = g.Key,
                    CantidadOrdenes = g.Count()
                })
                .ToListAsync();

            return Ok(resultado);
        }

        [HttpGet]
        public async Task<ActionResult<List<TiempoPromedioProduccionReporteDTO>>> ObtenerReporteTiempoPromedio()
        {
            var resultado = await _context.OrdenProduccion
                .GroupBy(o => o.Producto.Nombre) 
                .Select(g => new TiempoPromedioProduccionReporteDTO
                {
                    Producto = g.Key,
                    TiempoPromedioHoras = g.Average(o => EF.Functions.DateDiffHour(o.FechaInicio, o.FechaEntrega)) // Calcula el tiempo promedio de cada producto
                })
                .ToListAsync();

            return Ok(resultado);
        }

        public IActionResult GenerarPDF(string mes, string year)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // Validación de parámetros
            if (!int.TryParse(mes, out int month) || month < 1 || month > 12)
            {
                return BadRequest("Mes inválido");
            }

            if (!int.TryParse(year, out int yearNum))
            {
                return BadRequest("Año inválido");
            }

            // Consulta principal mapeando a OrdenReportePdfDto
            var datosReporte = _context.OrdenProduccion
                .Where(o => o.FechaInicio.Month == month && o.FechaInicio.Year == yearNum)
                .Where(o => new[] { "Finalizada", "En proceso", "Cancelada", "Pendiente" }.Contains(o.Estado))
                .Include(o => o.Producto)
                .GroupJoin(
                    _context.Reporte,
                    orden => orden.Ordenid,
                    reporte => reporte.OrdenId,
                    (orden, reportes) => new { orden, reporte = reportes.FirstOrDefault() }
                )
                .Select(x => new OrdenReportePdfDto
                {
                    OrdenId = x.orden.Ordenid,
                    CodigoProducto = x.orden.Producto.Codigo,
                    NombreProducto = x.orden.Producto.Nombre,
                    Cantidad = x.orden.Cantidad,
                    Estado = x.orden.Estado,
                    FechaInicio = x.orden.FechaInicio,
                    FechaEntrega = x.orden.FechaEntrega,
                    TiempoProduccion = x.reporte != null ? x.reporte.TiempoProduccion : null,
                    FechaGeneracion = x.reporte != null ? x.reporte.FechaGeneracion : null
                })
                .OrderBy(x => x.FechaInicio)
                .ToList();

            //PDF
            var logo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/caobalogo.png");
            var monedaGuatemala = new System.Globalization.CultureInfo("es-GT");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // Encabezado (logo y título)
                    page.Header().Column(header =>
                    {
                        header.Item().Row(row =>
                        {
                            row.ConstantItem(80).PaddingLeft(10).Image(logo, ImageScaling.FitArea);
                        });
                        header.Item().PaddingTop(10, Unit.Millimetre).AlignCenter()
                        .Text("REPORTE DE PRODUCCIÓN").FontSize(20).Bold();

                        header.Item().PaddingBottom(20);
                    });

                    page.Content().Column(column =>
                    {
                        // Información del período y fecha de generación
                        column.Item().PaddingVertical(10).Row(row =>
                        {
                            row.RelativeItem().Text($"Período: {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)} {yearNum}").FontSize(12);
                            column.Item().PaddingBottom(10);
                            row.RelativeItem().AlignRight().Text($"Fecha generación: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(12);
                        });

                        // --- ESPACIO ---
                        column.Item().PaddingVertical(10).Text("ÓRDENES DEL MES").FontSize(14).Bold();

                        column.Item().PaddingBottom(20);

                        column.Item().Table(table =>
                        {
                            column.Item().PaddingVertical(5, Unit.Millimetre);

                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(); // ID
                                columns.RelativeColumn(); // Código
                                columns.RelativeColumn(2); // Nombre
                                columns.RelativeColumn(); // Cantidad
                                columns.RelativeColumn(); // Estado
                                columns.RelativeColumn(); // Inicio
                                columns.RelativeColumn(); // Entrega
                                columns.RelativeColumn(); // Tiempo Producción
                                columns.RelativeColumn(); // Fecha Reporte
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(EstiloCelda).Text("ID").Bold();
                                header.Cell().Element(EstiloCelda).Text("Código").Bold();
                                header.Cell().Element(EstiloCelda).Text("Producto").Bold();
                                header.Cell().Element(EstiloCelda).Text("Cantidad").Bold();
                                header.Cell().Element(EstiloCelda).Text("Estado").Bold();
                                header.Cell().Element(EstiloCelda).Text("Inicio").Bold();
                                header.Cell().Element(EstiloCelda).Text("Entrega").Bold();
                                header.Cell().Element(EstiloCelda).Text("T.Prod").Bold();
                                header.Cell().Element(EstiloCelda).Text("F.Rep").Bold();
                            });

                            foreach (var o in datosReporte)
                            {
                                table.Cell().Element(EstiloCelda).Text(o.OrdenId);
                                table.Cell().Element(EstiloCelda).Text(o.CodigoProducto ?? "-");
                                table.Cell().Element(EstiloCelda).Text(o.NombreProducto ?? "-");
                                table.Cell().Element(EstiloCelda).Text(o.Cantidad.ToString());
                                table.Cell().Element(EstiloCelda).Text(o.Estado);
                                table.Cell().Element(EstiloCelda).Text(o.FechaInicio.ToShortDateString());
                                table.Cell().Element(EstiloCelda).Text(o.FechaEntrega?.ToShortDateString() ?? "-");
                                table.Cell().Element(EstiloCelda).Text(o.TiempoProduccion?.ToString("F2") ?? "-");
                                table.Cell().Element(EstiloCelda).Text(o.FechaGeneracion?.ToString("dd/MM/yyyy") ?? "-");
                            }
                        });
                    });

                    // Pie de página
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

            static IContainer EstiloCelda(IContainer container)
            {
                return container.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5);
            }

            var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;
            return File(stream, "application/pdf", $"ReporteProduccion_{month}_{yearNum}.pdf");
        }
    }
}

