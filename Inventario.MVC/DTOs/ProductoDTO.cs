using System.ComponentModel.DataAnnotations;

namespace Inventario.MVC.DTOs;

public class ProductoDTO
{
  public int Id { get; set; }
  public string Nombre { get; set; } = default!;
  public string Descripcion { get; set; } = default!;
  public float Precio { get; set; }
  public int Cantidad { get; set; }

  public int CategoriaID { get; set; }
  public CategoriaDTO Categoria { get; set; }

  public string CodigoBarra { get; set; }
  public string? ImagenUrl { get; set; }

  public DateTime FechaIngreso { get; set; }

  public bool Eliminado { get; set; }
}

public class ProductoCreateDTO
{
  [Required(ErrorMessage = "El {0} es requerido")]
  [StringLength(100, ErrorMessage = "El {0} no puede tener más de {1} caracteres.")] 
  public string Nombre { get; set; } = default!;

  [Required(ErrorMessage = "La descripcion es requerida")]
  [StringLength(500, ErrorMessage = "La descripción no puede tener más de {1} caracteres.")]
  public string Descripcion { get; set; } = default!;

  [Required(ErrorMessage = "El precio es requerido")]
  [Range(0.01f, float.MaxValue, ErrorMessage = "El precio debe ser mayor que cero.")]
  public float Precio { get; set; }

  // [Required(ErrorMessage = "La cantidad es requerida")]
  // [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")]
  public int Cantidad { get; set; } = 0; // creo que deberia ser cero inicialmente, esto se modificaria con las entradas y salidas

  [Required(ErrorMessage = "El codigo de barra es requerido")]
  // [RegularExpression(@"^\d{8,13}$", ErrorMessage = "El código de barra debe tener entre 8 y 13 dígitos.")]

  public string CodigoBarra { get; set; } = default!;

  [Url(ErrorMessage = "La imagen debe ser una URL válida")]
  public string? ImagenUrl { get; set; }

  [Required(ErrorMessage = "La categoria es requerida")]
  [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "La categoria es requerida")]
  public int CategoriaID { get; set; }
}

public class ProductoUpdateDTO : ProductoCreateDTO
{
  public int Id { get; set; }
}