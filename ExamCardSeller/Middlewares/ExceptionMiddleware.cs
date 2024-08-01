using ExamCardSeller.Infrastructure.Gateways;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
namespace ExamCardSeller.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred.");
                SentrySdk.CaptureException(ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var statusCode = HttpStatusCode.InternalServerError;
            var title = "An unexpected error occurred.";
            var detail = exception.Message;

            ProblemDetails problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            switch (exception)
            {
                case PaystackException paystackException:
                    statusCode = paystackException.StatusCode;
                    title = paystackException.Message;
                    problemDetails.Status = (int)statusCode;
                    problemDetails.Title = title;
                    break;
                case ValidationException validationException:
                    statusCode = HttpStatusCode.BadRequest;
                    title = "Validation failed.";
                    detail = FormatValidationErrors(validationException.Errors);
                    problemDetails.Status = (int)statusCode;
                    problemDetails.Title = title;
                    problemDetails.Detail = detail;
                    break;
                case BadHttpRequestException badHttpRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    title = "Bad Request";
                    detail = badHttpRequestException.Message;
                    problemDetails.Status = (int)statusCode;
                    problemDetails.Title = title;
                    problemDetails.Detail = detail;
                    break;
                    // Handle other custom exceptions here...
            }

            context.Response.StatusCode = (int)statusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonResponse = JsonSerializer.Serialize(problemDetails, options);
            return context.Response.WriteAsync(jsonResponse);
        }

        private static string FormatValidationErrors(IEnumerable<FluentValidation.Results.ValidationFailure> errors)
        {
            var errorMessages = new List<string>();
            foreach (var error in errors)
            {
                errorMessages.Add($"{error.PropertyName}: {error.ErrorMessage}");
            }
            return string.Join("; ", errorMessages);
        }
    }

}
