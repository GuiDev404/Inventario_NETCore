using Inventario.MVC.DTOs;
using Inventario.MVC.Models;

namespace Inventario.MVC.Interfaces;

public interface ICategoriaMapper {
  IEnumerable<CategoriaDTO> Map(IEnumerable<Categoria> source);
  CategoriaDTO Map(Categoria source);
  Categoria Map(CategoriaDTO source);

  Categoria Map(CategoriaCreateDTO source);

  void Map(Categoria result, CategoriaUpdateDTO dto);
}