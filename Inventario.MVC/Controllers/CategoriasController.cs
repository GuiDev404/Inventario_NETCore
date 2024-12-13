using Inventario.MVC.Data;
using Inventario.MVC.DTOs;
using Inventario.MVC.Filters;
using Inventario.MVC.Interfaces;
using Inventario.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.MVC.Controllers;

public class CategoriasController : Controller {
    private readonly ApplicationDbContext _context;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly ICategoriaMapper _categoriaMapper;

    public CategoriasController(ICategoriaRepository categoriaRepository, ICategoriaMapper categoriaMapper, ApplicationDbContext context)
    {
        _categoriaRepository = categoriaRepository;
        _categoriaMapper = categoriaMapper;
        _context = context;
    }

    public IActionResult Index() {
      return View();
    }

    public async Task<IActionResult> GetCategorias(){
      var categorias = await _categoriaRepository.GetCategories();
      var categoriasDTO = _categoriaMapper.Map(categorias);

      return Ok(new ApiResponse<IEnumerable<CategoriaDTO>> {
        Data = categoriasDTO,
        Message = "Listado de categorias obtenido correctamente",
        Success = true
      });
    }
    
    public async Task<IActionResult> GetCategoriaById(int id){
      Categoria? categoria = await _categoriaRepository.GetCategoryById(id);
      
      if (categoria is null){
        return NotFound(new ApiResponse<CategoriaDTO?> {
          Data = null,
          Message = "No se encontro ninguna categoria",
          Success = false
        });
      }

      var categoriaDTO = _categoriaMapper.Map(categoria);

      return Ok(new ApiResponse<CategoriaDTO> {
        Data = categoriaDTO,
        Message = "Categoria obtenida correctamente",
        Success = true
      });
    }

    [ServiceFilter(typeof(FiltroValidacionModelos))]
    public async Task<IActionResult> CreateCategoria(CategoriaCreateDTO categoriaCreateDTO) {
      bool alreadyExist = await _categoriaRepository.ExistCategoryByName(categoriaCreateDTO.Nombre);

      if (alreadyExist){
        return Conflict(new ApiResponse<object?>{
          Message = $"Ya existe una categoria con el nombre {categoriaCreateDTO.Nombre}",
          Success = false
        });
      }

      Categoria categoria = _categoriaMapper.Map(categoriaCreateDTO);
      bool categoriaIsSaved = await _categoriaRepository.CreateCategory(categoria);

      if (!categoriaIsSaved) {
        return StatusCode(
          StatusCodes.Status500InternalServerError,
          new ApiResponse<object?> { 
            Data = null,
            Message = "Algo salio mal, no se pudo crear la categoria",
            Success = false
          }
        );
      }

      return Ok(new ApiResponse<bool> {
        Data = true,
        Message = "Categoria creada correctamente",
        Success = true
      });
    }

    // [ServiceFilter(typeof(FiltroValidacionModelos))]
    public async Task<IActionResult> EditCategoria(int id, CategoriaUpdateDTO categoriaUpdateDTO) {

      bool alreadyExist = await _categoriaRepository.ExistCategoryByName(categoriaUpdateDTO.Nombre, id);

      if (alreadyExist){
        return Conflict(new ApiResponse<object?>{
          Message = $"Ya existe una categoria con el nombre {categoriaUpdateDTO.Nombre}",
          Success = false
        });
      }

      Categoria? categoriaFound = await _categoriaRepository.GetCategoryById(id);

      if (categoriaFound is null){
        return NotFound(new ApiResponse<Categoria?>() { 
          Success = false,
          Message = "Categoria no encontrada",
        });
      }

      _categoriaMapper.Map(categoriaFound, categoriaUpdateDTO);
      
      // Para que llegue la exception, en este caso categoria esta vacio
      _context.Categorias.Update(categoriaFound);
      await _context.SaveChangesAsync();

      // bool saved = await _categoriaRepository.UpdateCategory(categoriaFound);
      // if (!saved) {
      //   return BadRequest(new ApiResponse<CategoriaDTO?>() { 
      //     Success = false,
      //     Message = "No se pudo editar la categoria",
      //   });
      // }

      return Ok(new ApiResponse<CategoriaDTO?>() { 
        Success = true,
        Message = "Categoria actualizada exitosamente",
        Data = null
      });
    }

    public async Task<IActionResult> DeleteCategoria ([FromRoute] int id) {
      Categoria? categoria = await _categoriaRepository.GetCategoryById(id);

      if (categoria is null){
        return NotFound(new ApiResponse<object?>() { 
          Success = false,
          Message = "No se encontro la categoria a deshabilitar",
        });
      }

      bool existInProducto = await _categoriaRepository.ExistCategoriaInProducto(categoria.Id);
      if (existInProducto){
        return Conflict(new ApiResponse<object?>(){
          Success = false,
          Message = "La categoria existe en un producto, elimine primero este",
        });
      }

      bool result = await _categoriaRepository.ToggleCategoryAvailability(categoria, !categoria.Eliminado);

      if (!result){
        return BadRequest(new ApiResponse<object?>() { 
          Success = false,
          Message = "Algo salio mal, no se pudo eliminar la categoria",
        });
      }

      return NoContent();
    }
}