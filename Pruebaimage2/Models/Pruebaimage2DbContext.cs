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

    }

}
}
