using Inventario.MVC.DTOs;
using Inventario.MVC.Interfaces;
using Inventario.MVC.Models;

namespace Inventario.MVC.Mappers;

public class ProductoMapper : IProductoMapper
{
    public IEnumerable<ProductoDTO> Map(IEnumerable<Producto> source)
    {
      return source.Select(Map).ToList();
    }

    public ProductoDTO Map(Producto source)
    {
      return new ProductoDTO(){
        Id = source.Id,
        Cantidad = source.Cantidad,
        CategoriaID = source.CategoriaID,
        Categoria = new CategoriaDTO {
          Id = source.CategoriaID,
          Nombre = source.Categoria.Nombre,
          Eliminado = source.Categoria.Eliminado
        },
        CodigoBarra = source.CodigoBarra,
        Descripcion = source.Descripcion,
        Nombre = source.Nombre,
        Eliminado = source.Eliminado,
        FechaIngreso = source.FechaIngreso,
        ImagenUrl = source.ImagenUrl,
        Precio = source.Precio,
      };
    }

    public Producto Map(ProductoDTO source)
    {
      return new () {
        Id = source.Id,
        Eliminado = source.Eliminado,
        Nombre = source.Nombre,
        CodigoBarra = source.CodigoBarra,
        Precio = source.Precio,
        FechaIngreso = source.FechaIngreso,
        ImagenUrl = source.ImagenUrl, 
        Descripcion = source.Descripcion,
        Cantidad = source.Cantidad,
        CategoriaID = source.CategoriaID
      };
    }

    public Producto Map(ProductoCreateDTO source)
    {
      return new () {
        Nombre = source.Nombre,
        CodigoBarra = source.CodigoBarra.ToUpper(),
        Precio = source.Precio,
        ImagenUrl = source.ImagenUrl, 
        Descripcion = source.Descripcion,
        Cantidad = source.Cantidad,
        CategoriaID = source.CategoriaID
      };
    }

    public void Map(Producto result, ProductoUpdateDTO dto)
    {
      result.Nombre = dto.Nombre;
      result.CodigoBarra = dto.CodigoBarra.ToUpper();
      result.Precio = dto.Precio;
      result.ImagenUrl = dto.ImagenUrl;
      result.Descripcion = dto.Descripcion;
      result.Cantidad = dto.Cantidad;
      result.CategoriaID = dto.CategoriaID;
    }
} 