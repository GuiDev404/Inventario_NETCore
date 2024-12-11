using Inventario.MVC.Controllers;
using Inventario.MVC.DTOs;
using Inventario.MVC.Interfaces;
using Inventario.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Inventario.Tests.Controllers;

public class MovimientosControllerTests
{
  private readonly Mock<IMovimientoRepository> _movimientoRepositoryMock;
  private readonly Mock<IMovimientoMapper> _movimientoMapperMock;

  public MovimientosControllerTests()
  {
    _movimientoRepositoryMock = new Mock<IMovimientoRepository>(MockBehavior.Strict);
    _movimientoMapperMock = new Mock<IMovimientoMapper>(MockBehavior.Strict);
  }

  [Fact]
  public void Index_ReturnsAViewResult()
  {
    var movimientosController = new MovimientosController(
      _movimientoRepositoryMock.Object,
      _movimientoMapperMock.Object
    );

    var result = movimientosController.Index();

    var viewResult = Assert.IsType<ViewResult>(result);
    Assert.NotNull(viewResult); // Asegura que se devuelva una vista
  }

  [Fact]
  public async Task NuevoMovimiento_ShouldReturnOk() {
    var movimientoCreateDTO =  new MovimientoCreateDTO(){
      Cantidad = 5,
      ProductoId = 2,
      TipoMovimiento = TipoMovimiento.Entrada
    };
    
    Movimiento? movimiento =  new Movimiento(){
      Cantidad = 5,
      Stock = 5,
      ProductoId = 2,
      TipoMovimiento = TipoMovimiento.Entrada,
    };

    MovimientoDTO movimientoDTO = new (){
      Id = 1,
      Cantidad = 5,
      Stock = 5,
      Eliminado = false,
      Fecha = DateTime.Now,
      Producto = new ProductoDTO { Id = 2 },
      TipoMovimiento = TipoMovimiento.Entrada,
    };

    _movimientoMapperMock.Setup(m=> m.Map(movimientoCreateDTO)).Returns(movimiento);
    _movimientoRepositoryMock.Setup(r=> r.NewMovimiento(movimiento)).ReturnsAsync(movimiento);

    _movimientoMapperMock.Setup(m=> m.Map(movimiento)).Returns(movimientoDTO);
    
    var movimientosController = new MovimientosController(
      _movimientoRepositoryMock.Object,
      _movimientoMapperMock.Object
    );

    // Act
    var result = await movimientosController.NuevoMovimiento(movimientoCreateDTO);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var response = Assert.IsType<ApiResponse<MovimientoDTO>>(okResult.Value);

    Assert.True(response.Success);
    
    Assert.NotNull(response.Data);
    Assert.Equal(movimientoDTO.Id, response.Data.Id);
  }

  [Fact]
  public async Task GetMovimientos_ShouldReturnOkWithListOfMoves()
  {
    List<Movimiento> ls = new List<Movimiento>();
    List<MovimientoDTO> lsDTO = new List<MovimientoDTO>();

    _movimientoRepositoryMock.Setup((r)=> r.GetMovimientos()).ReturnsAsync(ls);
    _movimientoMapperMock.Setup(m=> m.Map(ls)).Returns(lsDTO);

    var movimientosController = new MovimientosController(
      _movimientoRepositoryMock.Object,
      _movimientoMapperMock.Object
    );

    // Act
    var result = await movimientosController.GetMovimientos();

    // Assert
    var actionResult = Assert.IsType<OkObjectResult>(result);;
    var response = Assert.IsType<ApiResponse<List<MovimientoDTO>>>(actionResult.Value);
    
    Assert.True(response.Success);
    Assert.NotNull(response.Data);
  }
}