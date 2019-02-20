using System;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;

namespace RadonTestsManager.Utility {
    public class CorrelationIdHeaderMiddleware {
        private const string CorrelationHeaderKey = "X-Correlation-ID";

        private readonly RequestDelegate _next;

        public CorrelationIdHeaderMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HttpContext context) {
            var requestTelemetry = context.Features.Get<RequestTelemetry>();

            context.Response.OnStarting(_ => {
                context.Response.Headers.Add(CorrelationHeaderKey, new[] { requestTelemetry.Id });
                return Task.CompletedTask;
            }, null);
            await _next(context);
        }
    }
}
