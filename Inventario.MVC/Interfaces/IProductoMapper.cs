using Inventario.MVC.DTOs;
using Inventario.MVC.Models;

namespace Inventario.MVC.Interfaces;

public interface IProductoMapper {
  IEnumerable<ProductoDTO> Map(IEnumerable<Producto> source);
  ProductoDTO Map(Producto source);
  Producto Map(ProductoDTO source);

  Producto Map(ProductoCreateDTO source);

  void Map(Producto result, ProductoUpdateDTO dto);
}