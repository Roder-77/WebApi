using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Responses;
using System.Net;

namespace WebApi.Filters
{
    public class LogInvalidModelState : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var logger = context.HttpContext.RequestServices.GetService<ILogger<LogInvalidModelState>>()!;
                var errors = context.ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage);
                var message = string.Join(", ", errors);
                var url = context.HttpContext.Request.GetDisplayUrl();

                logger.LogError("HttpMethod: {HttpMethod}, Url: {Url}, Message: {Message}", context.HttpContext.Request.Method, url, message);

                var result = new Response<object> { Code = 0, Message = message };
                context.Result = new ObjectResult(result) { StatusCode = (int)HttpStatusCode.BadRequest };

                return;
            }

            await next();
        }
    }
}