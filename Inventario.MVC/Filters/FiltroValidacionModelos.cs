using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inventario.MVC.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Inventario.MVC.Filters
{
    public class FiltroValidacionModelos : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var ModelState = context.ModelState;

            if (!ModelState.IsValid) {
                var errors = ModelState
                    .Where(m => m.Value.Errors.Any())
                    .Select(m => new {
                        Field = m.Key,
                        Messages = m.Value.Errors.Select(e => e.ErrorMessage)
                    })
                    .ToList();
                
                context.Result = new BadRequestObjectResult(new ApiResponse<object> {
                    Success = false,
                    Message = "Complete los campos requeridos",
                    Data = errors
                }); 

                return;
            }

            await next();
        }
    }
}