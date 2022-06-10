using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Response;
using System.Net;

namespace WebApi.Filters
{
    public class LogInvalidModelState : ActionFilterAttribute
    {
        private ILogger<LogInvalidModelState> _logger;

        public LogInvalidModelState(ILogger<LogInvalidModelState> logger)
        {
            _logger = logger;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage);

                var message = string.Join(", ", errors);

                _logger.LogError(message);

                var result = new Response<object> { Code = 0, Message = message };
                context.Result = new ObjectResult(result) { StatusCode = (int)HttpStatusCode.BadRequest };

                return;
            }


            await next();
        }

    }
}
