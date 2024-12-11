using Inventario.MVC.Models;

namespace Inventario.MVC.Interfaces;

public interface ICategoriaRepository
{
  Task<IEnumerable<Categoria>> GetCategories();
  // Task<IEnumerable<Producto>?> GetProductsByCategoria(int categoryId);
  Task<Categoria?> GetCategoryById(int id);
  Task<bool> ExistCategoryByName(string name, int? id = null);
  Task<bool> ExistCategoriaInProducto(int categoryId);
  Task<bool> CreateCategory(Categoria category);
  Task<bool> UpdateCategory(Categoria category);
  Task<bool> ToggleCategoryAvailability(Categoria category, bool eliminado);
  Task<bool> Save();
}