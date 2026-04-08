using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class OrgIdLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<OrgIdLoggingMiddleware> _logger;

    public OrgIdLoggingMiddleware(RequestDelegate next, ILogger<OrgIdLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put || context.Request.Method == HttpMethods.Patch)
        {
            var requestBody = await ReadRequestBodyAsync(context.Request);
            if (requestBody != null)
            {
                using var jsonDoc = JsonDocument.Parse(requestBody);
                if (jsonDoc.RootElement.TryGetProperty("organizationId", out var orgId))
                {
                    _logger.LogInformation("OrganizationId: {orgId}<Middleware level>", orgId);
                }
            }
        }

        await _next(context);
    }

    private async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();
        var buffer = new byte[request.ContentLength ?? 0];
        int bytesRead = await request.Body.ReadAsync(buffer, 0, buffer.Length);
        var bodyAsText = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        request.Body.Position = 0;
        return bodyAsText;
    }

}
