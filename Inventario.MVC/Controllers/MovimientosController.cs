using Inventario.MVC.DTOs;
using Inventario.MVC.Filters;
using Inventario.MVC.Interfaces;
using Inventario.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.MVC.Controllers;

public class MovimientosController : Controller
{
    private readonly IMovimientoRepository _movimientoRepository;
    private readonly IMovimientoMapper _movimientoMapper;

    public MovimientosController(IMovimientoRepository movimientoRepository, IMovimientoMapper movimientoMapper)
    {
        _movimientoRepository = movimientoRepository;
        _movimientoMapper = movimientoMapper;
    }

    public IActionResult Index(){
    
      return View();
    }

    [ServiceFilter(typeof(FiltroValidacionModelos))]
    public async Task<IActionResult> NuevoMovimiento (MovimientoCreateDTO movimientoCreateDTO) {
      var movimiento = _movimientoMapper.Map(movimientoCreateDTO);
      Movimiento? movimientoSaved = await _movimientoRepository.NewMovimiento(movimiento);

      if (movimientoSaved is null) {
        return BadRequest(new ApiResponse<object?>{ 
          Success = false,
          Message = "Algo salio mal, no se pudo crear el movimiento",
          Data = null
        });
      }

      MovimientoDTO movimientoSavedDTO = _movimientoMapper.Map(movimientoSaved);

      return Ok(new ApiResponse<MovimientoDTO>{ 
        Success = true,
        Message = "Movimiento realizado exitosamente",
        Data = movimientoSavedDTO
      });
    }

    public async Task<IActionResult> GetMovimientos (){
      var movimientos = await _movimientoRepository.GetMovimientos();
      var movimientoDTOs =  _movimientoMapper.Map(movimientos);

      return Ok(new ApiResponse<List<MovimientoDTO>> {
        Success = true,
        Message = "Listado de movimientos",
        Data = movimientoDTOs
      });
    }
}