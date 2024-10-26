﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PeliculasIdentity.Data;
using PeliculasIdentity.Data.Entities;
using PeliculasIdentity.Data.Enums;
using PeliculasIdentity.Models;
using PeliculasIdentity.Services;

namespace PeliculasIdentity.Controllers
{
    public class AccountController : Controller
    {
        private readonly IServicioUsuario _usuario;
        private readonly DataContext _context;
        private readonly IServicioCorreo _correo;

        public AccountController(IServicioUsuario usuario, DataContext context, IServicioCorreo correo)
        {
            _usuario = usuario;
            _context = context;
            _correo = correo;
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
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Ha superado el máximo número de intentos, su cuenta está bloqueada, intente de nuevo en 5 minutos.");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "El usuario no ha sido habilitado, debes de seguir las instrucciones del correo enviado para poder habilitar el usuario.");
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

        public IActionResult NotAuthorized()
        {
            return View();
        }

        public IActionResult Register()
        {
            AddUserViewModel model = new()
            {
                Id = Guid.Empty.ToString(),
                Rol = Rol.User,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Usuario user = await _usuario.AddUserAsync(model);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Este correo ya está siendo usado.");
                    return View(model);
                }

                string myToken = await _usuario.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme)!;

                Response response = _correo.SendMail(
               $"{model.Nombre}",
                model.Username,
                "Tecnologers - Confirmación de Email",
                $"<h1>Tecnologers - Confirmación de Email</h1>" +
                $"Para habilitar el usuario por favor hacer clic en el siguiente link:, " +
                $"<p><a href = \"{tokenLink}\">Confirmar Email</a></p>");
                if (response.IsSuccess)
                {
                    ViewBag.Message = "Las instrucciones para habilitar el usuario han sido enviadas al correo.";
                    return View(model);
                }
                ModelState.AddModelError(string.Empty, response.Message!);
            }
            return View(model);
        }

        public async Task<IActionResult> ChangeUser()
        {
            Usuario user = await _usuario.GetUserAsync(User!.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }
            EditUserViewModel model = new()
            {
                FechaRegistro = user.FechaRegistro,
                Nombre = user.Nombre,
                Id = user.Id,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Usuario user = await _usuario.GetUserAsync(User.Identity!.Name!);
                user.Nombre = model.Nombre;
                user.FechaRegistro = model.FechaRegistro;

                await _usuario.UpdateUserAsync(user);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _usuario.GetUserAsync(User.Identity!.Name!);
                if (user != null)
                {
                    var result = await _usuario.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault()!.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }
            Usuario user = await _usuario.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }
            IdentityResult result = await _usuario.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }
            return View();
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                Usuario user = await _usuario.GetUserAsync(model.Email!);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "El email no corresponde a ningún usuario registrado.");
                    return View(model);
                }
                string myToken = await _usuario.GeneratePasswordResetTokenAsync(user);
                string tokenLink = Url.Action("ResetPassword", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme)!;

                Response response = _correo.SendMail(
               $"{user.Nombre}",
                model.Email!,
                "Tecnologers - Recuperacion de contrasena",
                $"<h1>Tecnologers - Recuperacion de contrasena</h1>" +
                $"Para habilitar el usuario por favor hacer clic en el siguiente link:, " +
                $"<p><a href = \"{tokenLink}\">Confirmar Email</a></p>");
                if (response.IsSuccess)
                {
                    ViewBag.Message = "Las instrucciones para recuperar su password han sido enviadas al correo.";
                    return View(model);
                }
                ModelState.AddModelError(string.Empty, response.Message!);
            }
            return View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            Usuario user = await _usuario.GetUserAsync(model.UserName!);
            if (user != null)
            {
                IdentityResult result = await _usuario.ResetPasswordAsync(user, model.Token!, model.Password!);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Contraseña cambiada con éxito.";
                    return View();
                }
                ViewBag.Message = "Error cambiando la contraseña.";
                return View(model);
            }
            ViewBag.Message = "Usuario no encontrado.";
            return View(model);
        }
    }
}