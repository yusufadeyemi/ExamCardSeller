using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
namespace ExamCardSeller.Middlewares
{


    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            var request = await FormatRequest(context.Request);
            _logger.LogInformation($"Incoming request: {request}");

            context.Request.Body.Position = 0;
            await _next(context);
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            var body = request.Body;

            // This condition ensures the stream is at the start
            body.Position = 0;

            // Reading the stream as text
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await body.ReadAsync(buffer, 0, buffer.Length);
            var requestBody = Encoding.UTF8.GetString(buffer);

            body.Position = 0;

            return $"{request.Method} {request.Path} {request.QueryString} {requestBody}";
        }
    }

}
