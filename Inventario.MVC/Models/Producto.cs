namespace Inventario.MVC.Models;

public class Producto : Base {
  public string Nombre { get; set; } = default!;

  public string Descripcion { get; set; } = default!;
  public float Precio { get; set; }
  public int Cantidad { get; set; } // Cantidad o Stock

  // Código único que puede ser escaneado para identificación rápida.
  public string CodigoBarra { get; set; } = "";
  
  // Enlace a una imagen del producto para mostrar en interfaces visuales.
  public string? ImagenUrl { get; set; }

  // Ayuda a rastrear cuándo se incorporó al inventario.
  public DateTime FechaIngreso { get; set; } = DateTime.Now;

  public int CategoriaID { get; set; }
  public Categoria Categoria { get; set; }

  public ICollection<Movimiento> Movimientos { get; set;}
}