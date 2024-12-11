using Inventario.MVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Inventario.MVC.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Movimiento> Movimientos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) {
        // Llama al método base para que configure las entidades de identidad.
        base.OnModelCreating(builder); 
    
        builder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nombre = "Bebidas", Eliminado = false },
            new Categoria { Id = 2, Nombre = "Snacks", Eliminado = false },
            new Categoria { Id = 3, Nombre = "Lácteos", Eliminado = false },
            new Categoria { Id = 4, Nombre = "Panadería", Eliminado = false },
            new Categoria { Id = 5, Nombre = "Frutas", Eliminado = false }
        );
    }
}
