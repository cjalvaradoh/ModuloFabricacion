using caobaModeloFabricacion.Data;
using caobaModeloFabricacion.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace caobaModeloFabricacion.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewData["Layout"] = "_LayoutLogged";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["Layout"] = "_Layout";
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Usuario);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Usuario!, model.Password!, false, false);
                    if (result.Succeeded)
                    {
                        ViewData["Layout"] = "_LayoutLogged";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Contraseña incorrecta.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "El usuario no existe.");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState inválido:");
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($"- Campo: {kvp.Key} → Error: {error.ErrorMessage}");
                    }
                }
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Usuario,
                Email = model.Email,
                Nombre = model.Nombre,
                Rol = model.Rol.ToString(),
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, model.Rol.ToString());

                if (!roleResult.Succeeded)
                {
                    Console.WriteLine("❌ Error al asignar rol:");
                    foreach (var error in roleResult.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                Console.WriteLine("✅ Usuario creado correctamente");
                return RedirectToAction("Login", "Auth");
            }

            Console.WriteLine("❌ Errores en CreateAsync:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"- {error.Description}");
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }
    }
}
