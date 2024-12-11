using Inventario.MVC.Models;

namespace Inventario.MVC.Interfaces;

public interface IProductoRepository
{
  Task<IEnumerable<Producto>> GetProducts();
  // Task<IEnumerable<Producto>?> GetProductsByCategoria(int categoryId);
  Task<Producto?> GetProductById(int id);
  Task<bool> ExistProductByBarcode(string codigoBarra, int? id = null);
  Task<bool> ExistProductInMovimiento(int productoId);
  Task<Producto?> CreateProduct(Producto producto);
  Task<bool> UpdateProduct(Producto producto);
  Task<bool> ToggleProductAvailability(Producto producto, bool Eliminado);
  Task<bool> Save();
}