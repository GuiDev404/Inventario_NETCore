using Inventario.MVC.Controllers;
using Inventario.MVC.DTOs;
using Inventario.MVC.Filters;
using Inventario.MVC.Interfaces;
using Inventario.MVC.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace Inventario.Tests.Controllers;

public class ProductosControllerTests
{
    private readonly Mock<IProductoRepository> _productoRepoMock;
    private readonly Mock<ICategoriaRepository> _categoriaRepoMock;
    private readonly Mock<IProductoMapper> _productoMapperMock;
    // private readonly ProductosController _controller;
    
    
    // Variables privadas para reutilizar en las pruebas
    private readonly ProductoCreateDTO productoCreateDTO;
    private readonly Producto newProducto;
    private readonly Producto productSaved;
    private readonly ProductoDTO productoDTO;

    public ProductosControllerTests()
    {
        _productoRepoMock = new Mock<IProductoRepository>(MockBehavior.Strict);
        _productoMapperMock = new Mock<IProductoMapper>(MockBehavior.Strict);
        _categoriaRepoMock = new Mock<ICategoriaRepository>(MockBehavior.Strict);

        // _controller = new ProductosController(_repoMock.Object, _mapperMock.Object);
        
        // Inicialización de las instancias que serán reutilizadas
        productoCreateDTO = new ProductoCreateDTO
        {
            Nombre = "Name Test Product",
            Precio = 1000,
            Cantidad = 5,
            CategoriaID = 1,
            CodigoBarra = "S54A5S12",
            Descripcion = "Desc Test Product",
            ImagenUrl = ""
        };

        newProducto = new Producto {
            Id = 0,
            Eliminado = false,
            FechaIngreso = DateTime.Now, 
            Nombre = "Name Test Product",
            Precio = 1000,
            Cantidad = 5,
            CategoriaID = 1,
            CodigoBarra = "S54A5S12",
            Descripcion = "Desc Test Product",
            ImagenUrl = ""
        };

        productSaved = newProducto;

        productoDTO = new ProductoDTO
        {
            Id = 1,
            Nombre = "Name Test Product",
            Precio = 1000,
            Cantidad = 5,
            CategoriaID = 1,
            CodigoBarra = "S54A5S12",
            Descripcion = "Desc Test Product",
            ImagenUrl = ""
        };
    }

    [Fact]
    public async Task Index_ReturnsAViewResult_WithAViewBagOfCategories (){
        // Arrange
        IEnumerable<Categoria> categorias = new List<Categoria> (){
            new () {  Id = 0, Nombre = "[SELECCIONE UNA CATEGORIA]", Eliminado = false },
            new () {  Id = 1, Nombre = "Bebidas", Eliminado = false },
        };

        _categoriaRepoMock
            .Setup(repo=> repo.GetCategories())
            .ReturnsAsync(categorias);

        var controller =  new ProductosController(
            _productoRepoMock.Object,
            _productoMapperMock.Object,
            _categoriaRepoMock.Object
        );
        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);

        Assert.NotNull(viewResult); // Asegura que se devuelva una vista
        Assert.NotNull(viewResult.ViewData); // Asegura que ViewData esté disponible
        Assert.True(viewResult.ViewData.ContainsKey("Categorias")); // Verifica si la clave existe
    }

    [Fact]
    public async Task FiltroValidacionModelos_ShouldReturnBadRequest_WhenModelStateIsInvalid() {
        // Arrange
        var filtro = new FiltroValidacionModelos();
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Nombre", "El nombre es requerido");

        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new Microsoft.AspNetCore.Routing.RouteData(),
            new ActionDescriptor(),
            modelState // Aquí se pasa el ModelState directamente
        );

        var context = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object>(),
            controller: null
        );

        var next = new ActionExecutionDelegate(() => Task.FromResult(new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), controller: null)));

        // Act
        await filtro.OnActionExecutionAsync(context, next);

        // Assert
        var result = Assert.IsType<BadRequestObjectResult>(context.Result);
        var response = Assert.IsType<ApiResponse<object>>(result.Value);

        Assert.False(response.Success);
        Assert.Equal("Complete los campos requeridos", response.Message);

        var errors = response.Data as IEnumerable<object>;
        Assert.NotEmpty(errors);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreatedResponse_WhenProductIsSaved()
    {
        // Arrange
        _productoMapperMock.Setup(m => m.Map(productoCreateDTO)).Returns(newProducto);

        _productoRepoMock.Setup(r => r.ExistProductByBarcode(newProducto.CodigoBarra, null)).ReturnsAsync(false);
        
        productSaved.Id = 1;
        _productoRepoMock.Setup(r => r.CreateProduct(newProducto)).ReturnsAsync(productSaved);

        _productoMapperMock.Setup(m => m.Map(newProducto)).Returns(productoDTO);

        var controller =  new ProductosController(
            _productoRepoMock.Object,
            _productoMapperMock.Object,
            _categoriaRepoMock.Object
        );

        // Act
        var result = await controller.CreateProduct(productoCreateDTO);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<ProductoDTO>>(okResult.Value);

        Assert.True(response.Success);
        Assert.Equal("Producto creado exitosamente", response.Message);
        Assert.NotNull(response.Data);

        // Verificar invocaciones de los mocks
        _productoRepoMock.Verify(r => r.ExistProductByBarcode(productoCreateDTO.CodigoBarra, null), Times.Once);
        _productoRepoMock.Verify(r => r.CreateProduct(newProducto), Times.Once);
        _productoMapperMock.Verify(m => m.Map(productoCreateDTO), Times.Once);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnsOk (){
        // Arrange
        IEnumerable<Producto> productos = new List<Producto>();
        IEnumerable<ProductoDTO> productosDTO = new List<ProductoDTO>();

        _productoRepoMock
            .Setup(r=> r.GetProducts())
            .ReturnsAsync(productos);

        _productoMapperMock
            .Setup(r=> r.Map(productos))
            .Returns(productosDTO);

        var controller =  new ProductosController(
            _productoRepoMock.Object,
            _productoMapperMock.Object,
            _categoriaRepoMock.Object
        );

        // Act
        var result = await controller.GetProducts();
        
        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);;
        var response = Assert.IsType<ApiResponse<IEnumerable<ProductoDTO>>>(actionResult.Value);
        
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Empty(response.Data);

        // Assert.Equal(1, response.Data.Count());
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNoContent_WhenIsOk (){
        // Arrange
        productSaved.Id = 1;
        _productoRepoMock
            .Setup(repo => repo.GetProductById(productSaved.Id) )
            .ReturnsAsync(productSaved);

        _productoRepoMock
            .Setup(repo=> repo.ToggleProductAvailability(productSaved, !productSaved.Eliminado))
            .ReturnsAsync(true);

        _productoRepoMock
            .Setup(repo=> repo.ExistProductInMovimiento(It.IsAny<int>()))
            .ReturnsAsync(false);

        var controller =  new ProductosController(
            _productoRepoMock.Object,
            _productoMapperMock.Object,
            _categoriaRepoMock.Object
        );

        // Act
        var result = await controller.DeleteProduct(productSaved.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _productoRepoMock
            .Verify(repo=> repo.ToggleProductAvailability(productSaved, !productSaved.Eliminado), Times.Once);
    }
}