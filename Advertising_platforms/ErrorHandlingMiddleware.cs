using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace Advertising_platforms;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        HttpStatusCode status;
        string message;

        // 🛠️ Определяем статус по типу ошибки
        switch (ex)
        {
            case ArgumentException:
            case InvalidOperationException:
                status = HttpStatusCode.BadRequest; // 400
                message = ex.Message;
                break;

            case KeyNotFoundException:
                status = HttpStatusCode.NotFound; // 404
                message = "Ресурс не найден";
                break;

            // можно добавить кастомные исключения
            case ValidationException vex:
                status = HttpStatusCode.BadRequest; // 400
                message = vex.Message;
                break;

            default:
                status = HttpStatusCode.InternalServerError; // 500
                message = "Внутренняя ошибка сервера";
                break;
        }

        var result = JsonSerializer.Serialize(new
        {
            status = (int)status,
            message
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        return context.Response.WriteAsync(result);
    }
}