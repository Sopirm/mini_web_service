using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Pr1.MinWebService.Domain;
using Pr1.MinWebService.Errors;
using Pr1.MinWebService.Middlewares;
using Pr1.MinWebService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Car Parts API",
        Version = "v1",
        Description = "API для управления каталогом автозапчастей"
    });
});

// настройка сериализации, чтобы ответы были компактнее
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddSingleton<IPartRepository, InMemoryPartRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = "swagger";
    });
}

// мидлвары для обработки запросов
app.UseMiddleware<RequestIdMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<TimingAndLogMiddleware>();

// чтение списка запчастей
app.MapGet("/api/items", (IPartRepository repo) =>
{
    return Results.Ok(repo.GetAll());
});

// чтение запчасти по идентификатору
app.MapGet("/api/items/{id:guid}", (Guid id, IPartRepository repo) =>
{
    var part = repo.GetById(id);
    if (part is null)
        throw new NotFoundException("Запчасть с id {id} не найдена");

    return Results.Ok(part);
});

// создание запчасти
app.MapPost("/api/items", (HttpContext ctx, CreatePartRequest request, IPartRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        throw new ValidationException("Название запчасти не должно быть пустым");

    if (request.Name.Length < 3)
        throw new ValidationException("Название запчасти слишком короткое (минимум 3 символа)");

    if (request.Name.Length > 100)
        throw new ValidationException("Название запчасти слишком длинное (максимум 100 символов)");

    if (request.Price <= 0)
        throw new ValidationException("Цена запчасти должна быть больше нуля");

    if (request.Price > 1_000_000)
        throw new ValidationException("Цена запчасти слишком велика (максимум 1 000 000)");

    if (!request.Name.Any(char.IsLetter))
        throw new ValidationException("Название запчасти должно содержать хотя бы одну букву");

    var created = repo.Create(request.Name.Trim(), request.Price);

    var location = $"/api/items/{created.Id}";
    ctx.Response.Headers.Location = location;

    return Results.Created(location, created);
});

app.Run();
