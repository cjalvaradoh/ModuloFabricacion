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
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Materials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _context.Material.FindAsync(id);
            if (material != null)
            {
                _context.Material.Remove(material);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaterialExists(int id)
        {
            return _context.Material.Any(e => e.MaterialId == id);
        }
    }
}