using Microsoft.AspNetCore.Http.Features;
using Serilog;

namespace MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Configs
{
    public static class RequestEnricher
    {
        public static void LogAdditionalInfo(
            IDiagnosticContext diagnosticContext,
            HttpContext httpContext
            )
        {
            // Get the IP from HttpContext
            var remoteIp = httpContext.Connection.RemoteIpAddress;

            // Check if IP is available
            if (remoteIp != null)
            {
                diagnosticContext.Set("ClientIP", remoteIp.ToString());
             
            }
            else
            {
                // If IP is not available directly, try to get it from headers
                var forwardedHeader = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(forwardedHeader))
                {
                    diagnosticContext.Set("ClientIP", forwardedHeader);
                }
                else
                {
                    diagnosticContext.Set("ClientIP", "UNKNOWN");
                }
            }
        }
    }
}
