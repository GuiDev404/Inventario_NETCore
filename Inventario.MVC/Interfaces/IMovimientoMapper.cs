using Inventario.MVC.DTOs;
using Inventario.MVC.Models;

namespace Inventario.MVC.Interfaces;

public interface IMovimientoMapper {
  List<MovimientoDTO> Map(List<Movimiento> source);
  MovimientoDTO Map(Movimiento source);
  // Movimiento Map(MovimientoDTO source);

  Movimiento Map(MovimientoCreateDTO source);

  // void Map(Movimiento result, MovimientoUpdateDTO dto);
}