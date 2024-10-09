using Microsoft.AspNetCore.Mvc;
using PeliculasIdentity.Data;
using PeliculasIdentity.Models;
using PeliculasIdentity.Services;

namespace PeliculasIdentity.Controllers
{
    public class AccountController : Controller
    {
        private readonly IServicioUsuario _usuario;
        private readonly DataContext _context;

        public AccountController(IServicioUsuario usuario, DataContext context)
        {
            _usuario = usuario;
            _context = context;
        }

        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _usuario.LoginAsync(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _usuario.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}