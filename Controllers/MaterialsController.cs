using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using caobaModeloFabricacion.Data;
using caobaModeloFabricacion.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Data.SqlClient;

namespace caobaModeloFabricacion.Controllers
{
    public class MaterialsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Cloudinary _cloudinary;

        public MaterialsController(AppDbContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        // GET: Materials
        public async Task<IActionResult> Index()
        {
            return View(await _context.Material.ToListAsync());
        }

        // GET: Materials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Material
                .FirstOrDefaultAsync(m => m.MaterialId == id);
            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        // GET: Materials/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Materials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaterialId,Codigo,Nombre,Descripcion,Stock,PrecioUnidad,Ancho,Largo,Alto,Tipo,ThumbnailUrl,FotoUrl")] Material material, IFormFile FotoUrl)
        {
            if (ModelState.IsValid)
            {
                if (FotoUrl != null)
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(FotoUrl.FileName, FotoUrl.OpenReadStream()),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill")
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    material.FotoUrl = uploadResult.SecureUrl.ToString();

                    var thumbnailParams = new Transformation().Width(150).Height(150).Crop("thumb");
                    material.ThumbnailUrl = _cloudinary.Api.UrlImgUp.Transform(thumbnailParams).BuildUrl(uploadResult.PublicId);
                }
                _context.Add(material);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(material);
        }

        // GET: Materials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Material.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }
            return View(material);
        }

        // POST: Materials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Material material, IFormFile FotoUrl)
        {

            var materialExistente = await _context.Material.FindAsync(material.MaterialId);

            if (materialExistente == null)
            {
                TempData["Error"] = "Material no encontrado";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar propiedades manualmente
                    materialExistente.Codigo = material.Codigo;
                    materialExistente.Nombre = material.Nombre;
                    materialExistente.Descripcion = material.Descripcion;
                    materialExistente.Stock = material.Stock;
                    materialExistente.PrecioUnidad = material.PrecioUnidad;
                    materialExistente.Ancho = material.Ancho;
                    materialExistente.Largo = material.Largo;
                    materialExistente.Alto = material.Alto;
                    materialExistente.Tipo = material.Tipo;
                    materialExistente.ThumbnailUrl = material.ThumbnailUrl;
                    materialExistente.FotoUrl = material.FotoUrl;

                    if (FotoUrl != null)
                    {
                        // Subir la nueva imagen a Cloudinary
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(FotoUrl.FileName, FotoUrl.OpenReadStream()),
                            Transformation = new Transformation().Width(500).Height(500).Crop("fill")
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        materialExistente.FotoUrl = uploadResult.SecureUrl.ToString();

                        // Generar la miniatura
                        var thumbnailParams = new Transformation().Width(150).Height(150).Crop("thumb");
                        materialExistente.ThumbnailUrl = _cloudinary.Api.UrlImgUp.Transform(thumbnailParams).BuildUrl(uploadResult.PublicId);
                    }


                    await _context.SaveChangesAsync();
                    TempData["Success"] = "¡Guardado correctamente!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error: {ex.Message}";
                }
            }
            else
            {
                TempData["Error"] = "Error en los datos";
            }

            return RedirectToAction(nameof(Index)); // Siempre redirige al Index
        }


        // GET: Materials/Delete/5
        [HttpGet]
        public async Task<IActionResult> CanDelete(int id)
        {
            // Verificar órdenes activas
            var tieneOrdenesActivas = await _context.OrdenMaterial
                .Where(om => om.Materialid == id)
                .Join(_context.OrdenProduccion,
                      om => om.Ordenid,
                      op => op.Ordenid,
                      (om, op) => op)
                .AnyAsync(op => op.Estado != "Finalizada" && op.Estado != "Cancelada");

            // Verificar si está usado en detalles de producto
            var usadoEnProductos = await _context.DetalleProductoMaterial
                .AnyAsync(dpm => dpm.MaterialId == id);

            return Json(new
            {
                canDelete = !tieneOrdenesActivas && !usadoEnProductos,
                message = tieneOrdenesActivas ?
                    "No se puede eliminar por órdenes activas" :
                    usadoEnProductos ?
                    "No se puede eliminar por uso en productos" :
                    "Puede eliminarse"
            });
        }

        // POST: Materials/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // 1. Verificar si el material existe
            var material = await _context.Material.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }

            // 2. Verificar órdenes activas (misma lógica que tu consulta SQL)
            var tieneOrdenesActivas = await _context.OrdenMaterial
                .Where(om => om.Materialid == id)
                .Join(_context.OrdenProduccion,
                      om => om.Ordenid,
                      op => op.Ordenid,
                      (om, op) => op)
                .AnyAsync(op => op.Estado != "Finalizada" && op.Estado != "Cancelada");

            if (tieneOrdenesActivas)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "No se puede eliminar el material porque está asociado a órdenes activas."
                });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 3. Eliminar PRIMERO todos los registros relacionados:

                // a) Relación con orden_materiales
                var ordenesMaterial = await _context.OrdenMaterial
                    .Where(om => om.Materialid == id)
                    .ToListAsync();
                _context.OrdenMaterial.RemoveRange(ordenesMaterial);

                // b) Relación con detalle_producto_material (NUEVO)
                var detallesProducto = await _context.DetalleProductoMaterial
                    .Where(dpm => dpm.MaterialId == id)
                    .ToListAsync();
                _context.DetalleProductoMaterial.RemoveRange(detallesProducto);

                // 4. FINALMENTE eliminar el material
                _context.Material.Remove(material);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Material eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();

                string errorMessage = ex.InnerException?.Message.Contains("REFERENCE constraint") ?? false ?
                    "No se puede eliminar el material porque está relacionado con otros registros no considerados." :
                    "Ocurrió un error al intentar eliminar el material.";

                return StatusCode(500, new
                {
                    success = false,
                    message = errorMessage,
                    detailedError = ex.InnerException?.Message
                });
            }
        }
    }
}