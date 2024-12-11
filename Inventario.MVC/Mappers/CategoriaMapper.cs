using Inventario.MVC.DTOs;
using Inventario.MVC.Interfaces;
using Inventario.MVC.Models;

namespace Inventario.MVC.Mappers;

public class CategoriaMapper : ICategoriaMapper
{
    public IEnumerable<CategoriaDTO> Map(IEnumerable<Categoria> source)
    {
      return source.Select(Map).ToList();
    }

    public CategoriaDTO Map(Categoria source)
    {
      return new () {
        Id = source.Id,
        Nombre = source.Nombre,
        Eliminado = source.Eliminado
      };
    }

    public Categoria Map(CategoriaDTO source)
    {
      return new () {
        Id = source.Id,
        Eliminado = source.Eliminado,
        Nombre = source.Nombre,
      };
    }

    public Categoria Map(CategoriaCreateDTO source)
    {
      return new () {
        Nombre = source.Nombre,
      };
    }

    public void Map(Categoria result, CategoriaUpdateDTO dto)
    {
      result.Nombre = dto.Nombre;
    }
} 