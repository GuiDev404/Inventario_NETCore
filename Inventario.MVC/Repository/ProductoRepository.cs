using Inventario.MVC.Data;
using Inventario.MVC.Interfaces;
using Inventario.MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario.MVC.Repository;

public class ProductoRepository : IProductoRepository
{
    private readonly ApplicationDbContext _context;

    public ProductoRepository(ApplicationDbContext context)
    {
      _context = context;  
    }

    public async Task<Producto?> CreateProduct(Producto producto)
    {
      await _context.AddAsync(producto);
      bool saved = await Save();

      if (saved) {
        // Cargar relaciones navegables directamente en la instancia rastreada
          await _context.Entry(producto)
            .Reference(p => p.Categoria)
            .LoadAsync();

        return producto;
      }

      return null;
    }

    public async Task<bool> ToggleProductAvailability(Producto producto, bool availability)
    {
      producto.Eliminado = availability;
      return await Save();
    }

    public async Task<bool> ExistProductByBarcode(string codigoBarra, int? id)
    {
      if (id is not null) {
        return await _context.Productos.AnyAsync(p=> p.CodigoBarra == codigoBarra && p.Id != id);
      }

      return await _context.Productos.AnyAsync(p=> p.CodigoBarra == codigoBarra);
    }

    public async Task<Producto?> GetProductById(int id)
    {
      return await _context.Productos
        .Include(p=> p.Categoria)
        .FirstOrDefaultAsync(p=> p.Id == id);
    }

    public async Task<IEnumerable<Producto>> GetProducts()
    {
      return await _context.Productos
        .Include(p=> p.Categoria)
        .ToListAsync();
    }

    public async Task<bool> Save()
    {
      try {
        // The task result contains the number of state entries written to the database.
        int result = await _context.SaveChangesAsync(); 
        return result > 0;
      } catch (System.Exception) {
        return false;
      }
    }

    public async Task<bool> UpdateProduct(Producto producto)
    {
      _context.Update(producto);
      return await Save();
    }

    public async Task<bool> ExistProductInMovimiento(int productoId)
    {
      return await _context.Movimientos.AnyAsync(m=> m.ProductoId == productoId);
    }
}