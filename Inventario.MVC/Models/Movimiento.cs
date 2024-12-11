namespace Inventario.MVC.Models;

public enum TipoMovimiento {
    Entrada,
    Salida
}

public class Movimiento : Base {
  public int ProductoId { get; set; }
  public Producto Producto { get; set; }

  public DateTime Fecha { get; set; } = DateTime.Now;

  public TipoMovimiento TipoMovimiento { get; set; }  

  public int Cantidad { get; set; }

  public int Stock { get; set; }
}