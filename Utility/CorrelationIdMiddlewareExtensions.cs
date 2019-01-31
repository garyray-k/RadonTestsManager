using System;
using Microsoft.AspNetCore.Builder;

namespace RadonTestsManager.Utility.Models {
    public static class CorrelationIdMiddlewareExtensions {
        public static void UseCorrelationIdHeader(this IApplicationBuilder app) {
            app.UseMiddleware<CorrelationIdHeaderMiddleware>();
        }
    }
}
