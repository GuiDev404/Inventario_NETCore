
using Inventario.MVC.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.MVC.Middlewares
{
  public class ExceptionHandlingMiddleware {
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (System.Exception exception)
      {
        _logger.LogError(exception, "Exception occured: {Message}", exception.Message);
        var response = new ApiResponse<string> {
          Success = false,
          Data = "Internal Server Error",
          Message = "Algo salio mal"
        };

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(response);
      }
    }

  }
}