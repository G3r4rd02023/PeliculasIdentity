using Microsoft.AspNetCore.Identity;
using PeliculasIdentity.Data.Entities;
using PeliculasIdentity.Models;

namespace PeliculasIdentity.Services
{
    public interface IServicioUsuario
    {
        Task<Usuario> GetUserAsync(string email);

        Task<IdentityResult> AddUserAsync(Usuario user, string password);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(Usuario user, string roleName);

        Task<bool> IsUserInRoleAsync(Usuario user, string roleName);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<Usuario> AddUserAsync(AddUserViewModel model);

        Task<IdentityResult> UpdateUserAsync(Usuario user);

        Task<IdentityResult> ChangePasswordAsync(Usuario user, string oldPassword, string newPassword);

        Task<string> GenerateEmailConfirmationTokenAsync(Usuario user);

        Task<IdentityResult> ConfirmEmailAsync(Usuario user, string token);

        Task<Usuario> GetUserAsync(Guid userId);

        Task<string> GeneratePasswordResetTokenAsync(Usuario user);

        Task<IdentityResult> ResetPasswordAsync(Usuario user, string token, string password);
    }
}