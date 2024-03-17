using Microsoft.EntityFrameworkCore;

namespace Pruebaimage2.Models
{
    public class Pruebaimage2DbContext:DbContext
    {
        public Pruebaimage2DbContext(DbContextOptions<Pruebaimage2DbContext> options) : base(options)
        {
        }

        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Carro> Carros { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario { Email = "root@gmail.com", Id = 1, Nombre = "root", Apellido = "admin", Estatus = 1, Rol = "Administrador", Password = "e10adc3949ba59abbe56e057f20f883e" }
            );
        }

    }

}

