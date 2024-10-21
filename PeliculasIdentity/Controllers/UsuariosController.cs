using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasIdentity.Data;
using PeliculasIdentity.Data.Entities;
using PeliculasIdentity.Data.Enums;
using PeliculasIdentity.Models;
using PeliculasIdentity.Services;

namespace PeliculasIdentity.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IServicioUsuario _usuario;
        private readonly DataContext _context;

        public UsuariosController(IServicioUsuario usuario, DataContext context)
        {
            _usuario = usuario;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        public IActionResult Create()
        {
            AddUserViewModel model = new()
            {
                Id = Guid.Empty.ToString(),
                Rol = Rol.Admin,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Usuario user = await _usuario.AddUserAsync(model);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Este correo ya está siendo usado.");
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}