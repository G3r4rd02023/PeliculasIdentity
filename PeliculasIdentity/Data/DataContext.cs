using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeliculasIdentity.Data.Entities;

namespace PeliculasIdentity.Data
{
    public class DataContext : IdentityDbContext<Usuario>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Pelicula> Peliculas { get; set; }
    }
}