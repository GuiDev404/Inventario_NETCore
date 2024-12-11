using Inventario.MVC.Data;
using Inventario.MVC.Interfaces;
using Inventario.MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventario.MVC.Repository;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly ApplicationDbContext _context;

    public CategoriaRepository(ApplicationDbContext context)
    {
      _context = context;  
    }

    public async Task<bool> CreateCategory(Categoria categoria)
    {
      await _context.AddAsync(categoria);
      return await Save();
    }

    public async Task<bool> ToggleCategoryAvailability(Categoria category, bool eliminado)
    {
      category.Eliminado = eliminado;
      return await Save();
    }

    public async Task<bool> ExistCategoryByName(string nombre, int? id)
    {
      if (id is not null) {
        return await _context.Categorias.AnyAsync(p=> p.Nombre == nombre && p.Id != id);
      }

      return await _context.Categorias.AnyAsync(p=> p.Nombre == nombre);
    }

    public async Task<Categoria?> GetCategoryById(int id)
    {
      return await _context.Categorias.FindAsync(id);
    }

    public async Task<IEnumerable<Categoria>> GetCategories()
    {
      return await _context.Categorias.ToListAsync();
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

    public async Task<bool> UpdateCategory(Categoria categoria)
    {
      _context.Update(categoria);
      return await Save();
    }

    public Task<bool> ExistCategoriaInProducto(int categoryId)
      => _context.Productos.AnyAsync(p=> p.CategoriaID == categoryId);
}