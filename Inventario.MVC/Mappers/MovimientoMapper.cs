using Inventario.MVC.DTOs;
using Inventario.MVC.Interfaces;
using Inventario.MVC.Models;

namespace Inventario.MVC.Mappers;

public class MovimientoMapper : IMovimientoMapper
{
    private readonly IProductoMapper _productoMapper;

    public MovimientoMapper(IProductoMapper productoMapper)
    {
        _productoMapper = productoMapper;
    }

    public List<MovimientoDTO> Map(List<Movimiento> source)
    {
      return source.Select(Map).ToList();
    }

    public MovimientoDTO Map(Movimiento source)
    {
      return new () {
        Id = source.Id,
        Cantidad = source.Cantidad,
        Fecha = source.Fecha,
        Producto =  _productoMapper.Map(source.Producto),
        TipoMovimiento = source.TipoMovimiento,
        Stock = source.Stock, 
        Eliminado = source.Eliminado,
      };
    }

    // public Movimiento Map(MovimientoDTO source)
    // {
    //   return new () {
    //     Id = source.Id,
    //     Eliminado = source.Eliminado,
    //     Nombre = source.Nombre,
    //   };
    // }

    public Movimiento Map(MovimientoCreateDTO source)
    {
      return new () {
        Cantidad = source.Cantidad,
        ProductoId = source.ProductoId,
        TipoMovimiento = source.TipoMovimiento,
      };
    }

    // public void Map(Categoria result, CategoriaUpdateDTO dto)
    // {
    //   result.Nombre = dto.Nombre;
    // }
} 