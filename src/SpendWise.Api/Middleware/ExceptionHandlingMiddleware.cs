using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Application.Common.Exceptions;

namespace SpendWise.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(e => e.ErrorMessage).ToArray());

            await WriteProblemAsync(context,
                new ValidationProblemDetails(errors)
                {
                    Title = "One or more validation errors occurred."
                },
                StatusCodes.Status400BadRequest);
        }
        catch (DomainRuleViolationException ex)
        {
            var problem = new ProblemDetails
            {
                Title = "Domain validation failed.",
                Detail = ex.Message
            };

            await WriteProblemAsync(
                context,
                problem,
                StatusCodes.Status400BadRequest);
        }
        catch (InvalidCredentialsException)
        {
            context.Response.Headers.CacheControl = "no-store";
            context.Response.Headers.Pragma = "no-cache";

            await WriteProblemAsync(
                context,
                new ProblemDetails
                {
                    Title = "Authentication failed.",
                    Detail = "Invalid username or password."
                },
                StatusCodes.Status401Unauthorized);
        }
        catch (UnauthenticatedUserException)
        {
            context.Response.Headers.CacheControl = "no-store";

            await WriteProblemAsync(
                context,
                new ProblemDetails
                {
                    Title = "Authentication required."
                },
                StatusCodes.Status401Unauthorized);
        }
        catch (NotFoundException ex)
        {
            await WriteProblemAsync(
                context,
                new ProblemDetails
                {
                    Title = "Resource not found.",
                    Detail = ex.Message
                },
                StatusCodes.Status404NotFound);
        }
        catch (ConcurrencyConflictException ex)
        {
            await WriteProblemAsync(
                context,
                new ProblemDetails
                {
                    Title = "Concurrency conflict.",
                    Detail = ex.Message
                },
                StatusCodes.Status409Conflict);
        }
        catch (ConflictException ex)
        {
            var problem = new ProblemDetails
            {
                Title = "Resource conflict.",
                Detail = ex.Message
            };
            
            await WriteProblemAsync(
                context,
                problem,
                StatusCodes.Status409Conflict);
        }
        catch (DomainOperationException ex)
        {
            _logger.LogError(ex, "Unhandled domain operation error.");

            await WriteProblemAsync(
                context,
                new ProblemDetails
                {
                    Title = "An unexpected error occurred."
                },
                StatusCodes.Status500InternalServerError);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // The client canceled the request.
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            await WriteProblemAsync(
                context,
                new ProblemDetails
                {
                    Title = "An unexpected error occurred."
                },
                StatusCodes.Status500InternalServerError);
        }
    }

    private async Task WriteProblemAsync<TProblem>(HttpContext context, TProblem problem, int statusCode) where TProblem : ProblemDetails
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning(
                "Cannot write ProblemDetails because response has already started. TraceId: {TraceId}",
                context.TraceIdentifier);

            return;
        }

        problem.Status = statusCode;
        problem.Instance = context.Request.Path;
        problem.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(
            problem,
            contentType: "application/problem+json",
            options: null,
            cancellationToken: context.RequestAborted);
    }
}