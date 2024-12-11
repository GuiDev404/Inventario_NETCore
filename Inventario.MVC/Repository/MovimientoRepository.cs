using Inventario.MVC.Data;
using Inventario.MVC.Interfaces;
using Inventario.MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario.MVC.Repository;

public class MovimientoRepository : IMovimientoRepository
{
    private readonly ApplicationDbContext _context;

    public MovimientoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Movimiento>> GetMovimientos()
      => await _context.Movimientos
        .Include(m=> m.Producto)
        .Include(m=> m.Producto.Categoria)
        .ToListAsync();

    public async Task<Movimiento?> NewMovimiento(Movimiento movimiento)
    {
      var producto = await _context.Productos.FindAsync(movimiento.ProductoId);
      if (producto is null) return null;
      
      producto.Cantidad = movimiento.TipoMovimiento == TipoMovimiento.Entrada 
        ? producto.Cantidad + movimiento.Cantidad
        : producto.Cantidad - movimiento.Cantidad;

      movimiento.Stock = producto.Cantidad;

      await _context.Movimientos.AddAsync(movimiento);

      bool saved = await Save();

      if (saved) {
        // Cargar relaciones navegables directamente en la instancia rastreada, esto es una carga explicita
        await _context.Entry(movimiento)
            .Reference(p => p.Producto) // <-- Reference trae solo un producto 
            .LoadAsync();

        if (movimiento.Producto != null) {
            await _context.Entry(movimiento.Producto)
                .Reference(c => c.Categoria) // Cargar la categoría relacionada
                .LoadAsync();
        }

        return movimiento;
      }
      return null;
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
}