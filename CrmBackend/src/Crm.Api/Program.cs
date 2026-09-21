using System.Text.Json.Serialization;
using Crm.Api.Middleware;
using Crm.Application;
using Crm.Infrastructure;
using Crm.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---- Services ----
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // Serialize enums as strings ("New", "Won") rather than integers.
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .ConfigureApiBehaviorOptions(o =>
    {
        // An unparseable enum ("status":"vip") fails during deserialization, which
        // yields a confusing "The dto field is required." Replace it with the real cause.
        o.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(kv => kv.Value?.Errors.Count > 0)
                .ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value!.Errors
                        .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                            ? "Malformed value — check the field's allowed values."
                            : e.ErrorMessage)
                        .ToArray());

            // Body failed to bind at all: name the likely cause instead of "dto is required".
            if (errors.TryGetValue("dto", out var dtoErrors) &&
                dtoErrors.Any(e => e.Contains("required", StringComparison.OrdinalIgnoreCase)))
            {
                errors.Remove("dto");
                errors["body"] = new[]
                {
                    "Request body could not be parsed. Check that enum fields use an allowed value and that types match."
                };
            }

            return new BadRequestObjectResult(new ValidationProblemDetails(errors)
            {
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Instance = context.HttpContext.Request.Path
            });
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Clean Architecture layer registration.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// CORS for a local frontend (adjust origins for production).
builder.Services.AddCors(options =>
    options.AddPolicy("frontend", p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:4200")));

var app = builder.Build();

// ---- Pipeline ----
// First in the chain, so it wraps everything downstream.
app.UseGlobalExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Apply pending migrations automatically in development.
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseCors("frontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
