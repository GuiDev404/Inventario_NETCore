public class CategoriaDTO {
  public int Id { get; set; }
  public string Nombre{ get; set; }
  public bool Eliminado { get; set; }
}

public class CategoriaCreateDTO {
  public string Nombre { get; set; }
}


public class CategoriaUpdateDTO : CategoriaCreateDTO {
  public int Id { get; set; }
}