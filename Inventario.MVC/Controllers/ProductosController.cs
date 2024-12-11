using Inventario.MVC.Data;
using Inventario.MVC.DTOs;
using Inventario.MVC.Interfaces;
using Inventario.MVC.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventario.MVC.Controllers;

public class ProductosController : Controller
{
  private readonly IProductoMapper _productoMapper;
  private readonly IProductoRepository _productoRepo;

  // private readonly ICategoriaMapper _categoriaMapper; 
  private readonly ICategoriaRepository _categoriaRepo;

  public ProductosController(IProductoRepository productoRepo, IProductoMapper productoMapper, ICategoriaRepository categoriaRepo)
  {
    _productoMapper = productoMapper;
    _productoRepo = productoRepo;

    // _categoriaMapper = categoriaMapper;
    _categoriaRepo = categoriaRepo;
  }

  public async Task<IActionResult> Index() {
    var categorias = await _categoriaRepo.GetCategories();

    categorias = categorias
      .Append(new Categoria { Id = 0, Nombre = "[SELECCIONE UNA CATEGORIA]" })
      .OrderBy(c=> c.Id)
      .ToList();

    ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre");

    // List<SelectListItem> tiposSelectItems = new () {
    //   new ("[SELECCIONE UN TIPO DE MOVIENTO]", "-1")
    // };
  
    // var tipoMovimientos = Enum.GetValues(typeof(TipoMovimiento)).Cast<TipoMovimiento>();

    // tiposSelectItems.AddRange(
    //   tipoMovimientos.Select(t=> new SelectListItem {
    //     Value = t.GetHashCode().ToString(),
    //     Text = t.ToString()
    //   })
    // );

    // ViewBag.TiposMovimientos = tiposSelectItems.OrderBy(t=> t.Value).ToList();

    // var productos = await _productoRepo.GetProducts();
    // productos = productos
    //   .Append(new Producto { Id = 0, Nombre = "[SELECCIONE UN PRODUCTO]" })
    //   .OrderBy(p=> p.Id)
    //   .ToList();

    // ViewBag.Productos = new SelectList(productos, "Id", "Nombre");

    return View();
  }

  public async Task<IActionResult> GetProducts (){
    var productos = await _productoRepo.GetProducts();
    var productosDTO = _productoMapper.Map(productos);

    return Ok(new ApiResponse<IEnumerable<ProductoDTO>> (){
      Success = true,
      Message = "Productos obtenidos correctamente",
      Data = productosDTO
    });
  }

  public async Task<IActionResult> GetProductById (int id){
    var producto = await _productoRepo.GetProductById(id);

    if (producto is null){
      return NotFound(new ApiResponse<ProductoDTO?> (){
        Success = false,
        Message = "No se pudo recuperar el producto",
      });
    }

    var productosDTO = _productoMapper.Map(producto);

    return Ok(new ApiResponse<ProductoDTO> (){
      Success = true,
      Message = "Producto obtenido correctamente",
      Data = productosDTO
    });
  }

  public async Task<IActionResult> CreateProduct(ProductoCreateDTO productoCreateDTO)
  {
    if (!ModelState.IsValid) {
        var errors = ModelState
          .Where(m => m.Value.Errors.Any())
          .Select(m => new {
            Field = m.Key,
            Messages = m.Value.Errors.Select(e => e.ErrorMessage)
          })
          .ToList();
        
        return BadRequest(new ApiResponse<object> {
          Success = false,
          Message = "Complete los campos requeridos",
          Data = errors
        });
    }

    bool alreadyExist = await _productoRepo.ExistProductByBarcode(productoCreateDTO.CodigoBarra);

    if (alreadyExist){
      return Conflict(new ApiResponse<object?> {
        Success = false,
        Message = "Ya existe un producto con el código de barra",
        Data = null
      });
    }

    var producto = _productoMapper.Map(productoCreateDTO);
    Producto? productoSaved = await _productoRepo.CreateProduct(producto);

    if (productoSaved is null) {
      return BadRequest(new ApiResponse<object?>{ 
        Success = false,
        Message = "Algo salio mal, no se pudo crear el producto",
        Data = null
      });
    }

    ProductoDTO productoSavedDTO = _productoMapper.Map(productoSaved);

    return Ok(new ApiResponse<ProductoDTO>{ 
      Success = true,
      Message = "Producto creado exitosamente",
      Data = productoSavedDTO
    });
  }

  public async Task<IActionResult> EditProduct([FromRoute] int id, ProductoUpdateDTO productoUpdateDTO)
  {
     if (!ModelState.IsValid) {
        var errors = ModelState
          .Where(m => m.Value.Errors.Any())
          .Select(m => new {
            Field = m.Key,
            Messages = m.Value.Errors.Select(e => e.ErrorMessage)
          })
          .ToList();
        
        return BadRequest(new ApiResponse<object> {
          Success = false,
          Message = "Complete los campos requeridos",
          Data = errors
        });
    }

    bool alreadyExist = await _productoRepo.ExistProductByBarcode(productoUpdateDTO.CodigoBarra, id);

    if (alreadyExist){
      return Conflict(new ApiResponse<ProductoDTO?>() { 
        Success = false,
        Message = "Ya existe un producto con el codigo de barra",
      });
    }

    Producto? productFound = await _productoRepo.GetProductById(id);

    if (productFound is null){
      return NotFound(new ApiResponse<Producto?>() { 
        Success = false,
        Message = "El producto a editar no existe",
      });
    }

    _productoMapper.Map(productFound, productoUpdateDTO);
    bool saved = await _productoRepo.UpdateProduct(productFound);
  
    if (!saved) {
      return BadRequest(new ApiResponse<ProductoDTO?>() { 
        Success = false,
        Message = "No se pudo editar el producto",
      });
    }

    // ProductoDTO newProductoDTO = _productoMapper.Map(productFound);
    
    return Ok(new ApiResponse<ProductoDTO?>() { 
      Success = true,
      Message = "Producto actualizado exitosamente",
      Data = null
    });
  }

  public async Task<IActionResult> DeleteProduct ([FromRoute] int id) {
    Producto? product = await _productoRepo.GetProductById(id);

    if (product is null){
      return NotFound(new ApiResponse<object?>() { 
        Success = false,
        Message = "El producto a eliminar no existe",
      });
    }

    bool existInMovimientos = await _productoRepo.ExistProductInMovimiento(product.Id);
    if (existInMovimientos){
      return Conflict(new ApiResponse<object?>(){
        Success = false,
        Message = "El producto existe en un movimiento, elimine primero este",
      });
    }

    bool result = await _productoRepo.ToggleProductAvailability(product, !product.Eliminado);

    if (!result){
      return BadRequest(new ApiResponse<object?>() { 
        Success = false,
        Message = "Algo salio mal, no se pudo eliminar el producto",
      });
    }

    return NoContent();
  }
}