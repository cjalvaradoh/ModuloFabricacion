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
            var reportData = _context.OrdenProduccion
                .Where(op => op.FechaInicio.Month == month && op.FechaInicio.Year == yearNum)
                .Include(op => op.Producto)
                .Include(op => op.Materiales)
                    .ThenInclude(om => om.Material)
                .Include(op => op.Seguimientos)
                    .ThenInclude(s => s.Operario)
                .Select(op => new OrdenReportePdfDto
                {
                    OrdenId = op.Ordenid,
                    Estado = op.Estado,
                    FechaInicio = op.FechaInicio,
                    FechaEntrega = op.FechaEntrega,
                    Producto = new ProductoDto
                    {
                        Codigo = op.Producto.Codigo,
                        Nombre = op.Producto.Nombre,
                        Descripcion = op.Producto.Descripcion
                    },
                    Materiales = op.Materiales.Select(om => new MaterialDto
                    {
                        Codigo = om.Material.Codigo,
                        Nombre = om.Material.Nombre,
                        Cantidad = om.CantidadUtilizada,
                        CostoUnitario = om.Material.PrecioUnidad
                    }).ToList(),
                    Seguimientos = op.Seguimientos.Select(s => new SeguimientoDto
                    {
                        OperarioNombre = s.Operario.Nombre,
                        OperarioCarnet = s.Operario.Carnet,
                        Avance = (decimal)s.Avance,
                        TiempoTrabajado = (decimal)s.TiempoTrabajado,
                        TarifaOperario = (decimal)s.Operario.TiempoHora,
                        FechaActualizacion = (DateTime)s.FechaActualizacion,
                        Estado = s.Estado
                    }).ToList()
                })
                .ToList();

            if (!reportData.Any())
            {
                return NotFound("No se encontraron órdenes para el período seleccionado");
            }

            //PDF
            var logo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/icons/apple-touch-icon-180x180.png");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Column(header =>
                    {
                        header.Item().PaddingVertical(5, Unit.Millimetre);
                        header.Item().Row(headerRow =>
                        {
                            headerRow.RelativeItem().Text("REPORTE DE PRODUCCIÓN").FontSize(20).Bold().AlignCenter();
                            headerRow.ConstantItem(100).Image(logo, ImageScaling.FitArea);
                        });
                    });

                    page.Content().Column(column =>
                    {
                        // Información principal del producto y orden
                        column.Item().PaddingVertical(10).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                column.Item().AlignCenter().Text($"Período: {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)} {yearNum}").FontSize(12);
                            });
                            row.RelativeItem().Column(col =>
                            {
                                column.Item().AlignCenter().Text($"Fecha generación: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10);
                            });
                            row.RelativeItem().Column(col =>
                            {
                                column.Item().PaddingBottom(10).LineHorizontal(1);
                            });
                        });

                        foreach (var orden in reportData)
                        {
                            column.Item().PaddingVertical(10).Column(orderColumn =>
                            {
                                // Datos básicos de la orden
                                orderColumn.Item().Row(row =>
                                {
                                    row.RelativeItem().Text($"Orden #: {orden.OrdenId}").Bold();
                                    row.RelativeItem().Text($"Estado: {orden.Estado}").AlignRight();
                                });

                                orderColumn.Item().Text($"Producto: {orden.Producto?.Codigo} - {orden.Producto?.Nombre}");
                                orderColumn.Item().Text($"Descripción: {orden.Producto?.Descripcion}");
                                orderColumn.Item().Row(row =>
                                {
                                    row.RelativeItem().Text($"Fecha inicio: {orden.FechaInicio:dd/MM/yyyy}");
                                    row.RelativeItem().Text(orden.FechaEntrega.HasValue ?
                                        $"Fecha entrega: {orden.FechaEntrega.Value:dd/MM/yyyy}" :
                                        "Fecha entrega: Pendiente").AlignRight();
                                });

                                // Progreso de producción
                                orderColumn.Item().PaddingVertical(5).Text("PROGRESO DE PRODUCCIÓN").FontSize(14).Bold();

                                orderColumn.Item().PaddingVertical(5).Row(row =>
                                {
                                    row.RelativeItem().Text("Progreso total:");
                                    row.RelativeItem().Text($"{orden.ProgresoPromedio:F1}%").AlignRight();
                                });


                                // Resumen de costos
                                orderColumn.Item().PaddingVertical(5).Row(row =>
                                {
                                    row.RelativeItem().Text("Costo materiales:");
                                    row.RelativeItem().Text($"{orden.CostoMateriales:C}").AlignRight();
                                });

                                orderColumn.Item().PaddingVertical(5).Row(row =>
                                {
                                    row.RelativeItem().Text("Costo mano de obra:");
                                    row.RelativeItem().Text($"{orden.CostoManoObra:C}").AlignRight();
                                });

                                orderColumn.Item().PaddingVertical(5).Row(row =>
                                {
                                    row.RelativeItem().Text("Costo total:").Bold();
                                    row.RelativeItem().Text($"{orden.CostoTotal:C}").Bold().AlignRight();
                                });

                                // Tabla de materiales
                                orderColumn.Item().PaddingVertical(10).Text("MATERIALES UTILIZADOS").FontSize(12).Bold();
                                orderColumn.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3); // Nombre
                                        columns.RelativeColumn();  // Código
                                        columns.RelativeColumn();  // Cantidad
                                        columns.RelativeColumn();  // Costo
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(EstiloCelda).Text("Material");
                                        header.Cell().Element(EstiloCelda).Text("Código");
                                        header.Cell().Element(EstiloCelda).Text("Cantidad");
                                        header.Cell().Element(EstiloCelda).Text("Costo").AlignRight();
                                    });

                                    foreach (var material in orden.Materiales)
                                    {
                                        table.Cell().Element(EstiloCelda).Text(material.Nombre ?? "N/A");
                                        table.Cell().Element(EstiloCelda).Text(material.Codigo ?? "N/A");
                                        table.Cell().Element(EstiloCelda).Text(material.Cantidad.ToString("F2"));
                                        table.Cell().Element(EstiloCelda).Text(material.Costo.ToString("C")).AlignRight();
                                    }
                                });

                                // Tabla de seguimientos
                                orderColumn.Item().PaddingVertical(10).Text("REGISTRO DE SEGUIMIENTO").FontSize(12).Bold();
                                orderColumn.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2); // Operario
                                        columns.RelativeColumn();  // Avance
                                        columns.RelativeColumn();  // Tiempo
                                        columns.RelativeColumn(2); // Fecha
                                        columns.RelativeColumn();  // Estado
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(EstiloCelda).Text("Operario");
                                        header.Cell().Element(EstiloCelda).Text("Avance %");
                                        header.Cell().Element(EstiloCelda).Text("Horas");
                                        header.Cell().Element(EstiloCelda).Text("Fecha");
                                        header.Cell().Element(EstiloCelda).Text("Estado");
                                    });

                                    foreach (var seg in orden.Seguimientos.OrderByDescending(s => s.FechaActualizacion))
                                    {
                                        table.Cell().Element(EstiloCelda).Text($"{seg.OperarioNombre} ({seg.OperarioCarnet})");
                                        table.Cell().Element(EstiloCelda).Text($"{seg.Avance:F1}%").AlignCenter();
                                        table.Cell().Element(EstiloCelda).Text($"{seg.TiempoTrabajado:F2}").AlignCenter();
                                        table.Cell().Element(EstiloCelda).Text(seg.FechaActualizacion.ToString("dd/MM/yyyy HH:mm")).AlignCenter();
                                        table.Cell().Element(EstiloCelda).Text(seg.Estado ?? "N/A").AlignCenter();
                                    }
                                });

                                orderColumn.Item().PaddingBottom(20).LineHorizontal(0.5f);
                            });
                        }
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

