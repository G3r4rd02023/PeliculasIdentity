using PeliculasIdentity.Data.Entities;
using PeliculasIdentity.Data.Enums;
using PeliculasIdentity.Services;

namespace PeliculasIdentity.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IServicioUsuario _usuario;

        public SeedDb(DataContext context, IServicioUsuario usuario)
        {
            _context = context;
            _usuario = usuario;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckMoviesAsync();
            await CheckRolesAsync();
            await CheckUserAsync("SuperAdmin", "tecnologershn@gmail.com", "123456");
        }

        private async Task CheckMoviesAsync()
        {
            if (!_context.Peliculas.Any())
            {
                _context.Peliculas.Add(new Pelicula
                {
                    Titulo = "Titanic",
                    Director = "James Cameron",
                    FechaPublicacion = DateTime.Today.AddYears(-25),
                    Genero = "Drama"
                });

                _context.Peliculas.Add(new Pelicula
                {
                    Titulo = "Alien",
                    Director = "Ridley Scott",
                    FechaPublicacion = DateTime.Today.AddYears(-35),
                    Genero = "Terror"
                });
            }
            await _context.SaveChangesAsync();
        }

        private async Task CheckRolesAsync()
        {
            await _usuario.CheckRoleAsync(Rol.Admin.ToString());
            await _usuario.CheckRoleAsync(Rol.User.ToString());
        }

        private async Task<Usuario> CheckUserAsync(string nombre, string correo, string password)
        {
            Usuario user = await _usuario.GetUserAsync(correo);
            if (user == null)
            {
                user = new Usuario
                {
                    Nombre = nombre,
                    UserName = correo,
                    Email = correo,
                    Rol = Rol.Admin,
                };
                await _usuario.AddUserAsync(user, password);
                await _usuario.AddUserToRoleAsync(user, Rol.Admin.ToString());
            }

            return user;
        }
    }
}