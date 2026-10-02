using BibliotecaMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BibliotecaMVC.Data
{
    public class BibliotecaContext : IdentityDbContext<IdentityUser>

    {

        public BibliotecaContext(DbContextOptions<BibliotecaContext> options) : base(options)
        {


        }

        public DbSet<Autor> Autores { get; set; }
        public DbSet<Libro> Libros { get; set; }
    }

}
