using Microsoft.AspNetCore.Mvc;
using caobaModeloFabricacion.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;


namespace caobaModeloFabricacion.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {

            var totalOrdenes = _context.OrdenProduccion.Count();
            var costoTotalProduccion = _context.OrdenMaterial
                .Sum(om => om.CantidadUtilizada * om.Material.PrecioUnidad);
            var totalHoras = _context.Operario
                .Where(op => op.TiempoHora.HasValue)
                .Sum(op => op.TiempoHora.Value);

            var totalOrdenesProduccion = _context.OrdenProduccion.Count();

            decimal eficiencia = 0;

            if (totalOrdenesProduccion > 0)
            {

                eficiencia = (totalHoras / totalOrdenesProduccion) * 100;
            }

            // gráfico de eficiencia por día (lunes a domingo)
            var eficienciaPorDia = _context.OrdenProduccion
                .AsEnumerable()
                .GroupBy(o => o.FechaInicio.DayOfWeek)
                .Select(g => new {
                    Dia = g.Key,
                    Eficiencia = g.Count() > 0
                        ? _context.Operario.Sum(op => op.TiempoHora ?? 0) / g.Count() * 100
                        : 0
                })
                .OrderBy(x => (int)x.Dia)
                .ToList();

            var dias = eficienciaPorDia
                .Select(x => CultureInfo.CurrentCulture.DateTimeFormat.GetShortestDayName(x.Dia).Substring(0, 1).ToUpper())
                .ToArray();

            var eficiencias = eficienciaPorDia.Select(x => Math.Round(x.Eficiencia, 2)).ToArray();

            var eficienciaChartData = new
            {
                labels = dias,
                datasets = new[]
                {
        new {
            label = "Eficiencia Diaria",
            data = eficiencias,
            backgroundColor = "rgba(255,165,0,0.2)",
            borderColor = "#ffa500",
            tension = 0.4,
            fill = true
        }
    }
            };

            ViewBag.EficienciaChart = JsonConvert.SerializeObject(eficienciaChartData);



            // tabla de operarios con datos reales
            var operarios = _context.Operario
                .AsEnumerable()
                .Select(op => new {
                    ImagenUrl = string.IsNullOrEmpty(op.ThumbnailUrl) ? "/images/default.png" : op.ThumbnailUrl,
                    Carnet = op.Carnet,
                    Nombre = op.Nombre,
                    Activo = op.Estado.ToLower() == "activo",
                    TiempoProduccion = op.TiempoHora.HasValue ? op.TiempoHora.Value.ToString("0.00") : "0.00",
                    Departamento = op.DepartamentoId.HasValue ? $"DEP{op.DepartamentoId.Value.ToString("000")}" : "Sin asignar"
                })
            .ToList();


            // datos para ordenes por mes
            var ordenesPorMes = _context.OrdenProduccion
                .AsEnumerable()
                .GroupBy(o => o.FechaInicio.ToString("MMMM", new CultureInfo("es-ES")))
                .Select(g => new {
                    Mes = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g.Key),
                    Cantidad = g.Count()
                })
                .OrderBy(g => DateTime.ParseExact(g.Mes, "MMMM", new CultureInfo("es-ES")))
                .ToList();

            var labels = ordenesPorMes.Select(x => x.Mes).ToArray();

            var data = ordenesPorMes.Select(x => x.Cantidad).ToArray();

            var chartData = new
            {
                labels = labels,
                datasets = new[]
                {
                    new {
                        label = "Órdenes",
                        data = data,
                        backgroundColor = "#4d2c26"
                    }
                }
            };

            ViewBag.TotalOrdenes = totalOrdenes;
            ViewBag.CostoTotalProduccion = costoTotalProduccion;
            ViewBag.EficienciaProduccion = Math.Round(eficiencia, 2);
            ViewBag.Operarios = operarios;
            ViewBag.OrdenesChart = JsonConvert.SerializeObject(chartData);

            // datos para costos por mes
            var costoPorMes = _context.OrdenMaterial
                .Include(om => om.Material)
                .Include(om => om.Orden)
                .AsEnumerable()
                .GroupBy(om => om.Orden.FechaInicio.ToString("MMMM", new CultureInfo("es-ES")))
                .Select(g => new
                {
                    Mes = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g.Key),
                    Total = g.Sum(om => om.CantidadUtilizada * om.Material.PrecioUnidad)
                })
                .OrderBy(g => DateTime.ParseExact(g.Mes, "MMMM", new CultureInfo("es-ES")))
                .ToList();

            var costoChartData = new
            {
                labels = costoPorMes.Select(x => x.Mes).ToArray(),
                datasets = new[]
                {
            new {
                label = "Costo Producción",
                data = costoPorMes.Select(x => x.Total).ToArray(),
                backgroundColor = "#f5a623"
                }
                }
            };

            ViewBag.CostoChart = JsonConvert.SerializeObject(costoChartData);


            return View();
        }
    }
}
