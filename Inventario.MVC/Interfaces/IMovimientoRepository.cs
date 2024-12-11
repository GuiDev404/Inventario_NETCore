using Inventario.MVC.Models;

namespace Inventario.MVC.Interfaces;

public interface IMovimientoRepository {
  Task<List<Movimiento>> GetMovimientos();
  Task<Movimiento?> NewMovimiento(Movimiento movimiento);

  Task<bool> Save();
}