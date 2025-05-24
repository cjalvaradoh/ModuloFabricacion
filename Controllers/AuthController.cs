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
                return RedirectToAction("Index", "Dashboard");
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
                        return RedirectToAction("Index", "Dashboard");
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
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Usuario,
                    Email = model.Email,
                    Nombre = model.Nombre,
                    Rol = model.Rol.ToString()
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Rol.ToString());
                    return RedirectToAction("Login", "Auth");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine(error.Description);
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
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
