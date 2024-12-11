using System.ComponentModel.DataAnnotations;
using Inventario.MVC.Models;

namespace Inventario.MVC.DTOs;

public class MovimientoDTO {
  public int Id { get; set; }

  public ProductoDTO Producto { get; set; }

  public DateTime Fecha { get; set; } = DateTime.Now;

  public TipoMovimiento TipoMovimiento { get; set; }  

  public int Cantidad { get; set; }
  public int Stock { get; set; }

  public bool Eliminado { get; set; }
}

public class MovimientoCreateDTO {
  public TipoMovimiento TipoMovimiento { get; set; }  

  [Required(ErrorMessage = "La cantidad es requerida")]
  [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")] 
  public int Cantidad { get; set; }

  public int ProductoId { get; set; }
}

public class MovimientoUpdateDTO : MovimientoCreateDTO {
  public int Id { get; set; }
}