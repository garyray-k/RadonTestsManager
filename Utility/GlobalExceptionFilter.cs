using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace RadonTestsManager.Utility {
    public class GlobalExceptionFilter : IExceptionFilter {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(IHostingEnvironment env, ILogger<GlobalExceptionFilter> logger) {
            _env = env;
            _logger = logger;
        }

        public void OnException(ExceptionContext context) {
            _logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);

            var errorDetails = new ErrorDetails {
                Instance = context.HttpContext.Request.Path,
                Status = StatusCodes.Status500InternalServerError,
                Detail = "Please refer to error property for additional details",
                Errors = {
                    { "ServerError", new[] {"An unexpected error occured."} }
                }
            };

            if (_env.IsDevelopment()) {
                errorDetails.Exception = context.Exception;
            }

            context.Result = new ObjectResult(errorDetails) {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            context.ExceptionHandled = true;
        }

        private class ErrorDetails : ValidationProblemDetails {
            public Exception Exception { get; set; }
        }
    }
}
