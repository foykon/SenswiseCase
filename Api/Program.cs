using Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Conn string
var conn = builder.Configuration.GetConnectionString("Postgres");

// DI: Infra + App
builder.Services.AddInfrastructure(conn);
builder.Services.AddApplication();

// Controllers + Swagger
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(opt =>
    {
        // Model binding hatalarýnda ProblemDetails (400)
        opt.InvalidModelStateResponseFactory = ctx =>
        {
            var pd = new ValidationProblemDetails(ctx.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Request validation failed"
            };
            return new BadRequestObjectResult(pd);
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
