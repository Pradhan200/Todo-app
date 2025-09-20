using FluentValidation;
using Serilog;
using TodoApp.Server.Middleware;
using TodoApp.Server.Models;
using TodoApp.Server.Services;
using TodoApp.Server.Validators;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Todo API", Version = "v1" });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateTodoRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

// Register repository
builder.Services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

// Add global exception handling
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors("AllowAngularApp");

// API Endpoints
app.MapGet("/api/todo", async (ITodoRepository repository) =>
{
    var todos = await repository.GetAllAsync();
    return Results.Ok(todos);
})
.WithName("GetTodos")
.WithOpenApi();

app.MapPost("/api/todo", async (CreateTodoRequest request, ITodoRepository repository) =>
{
    try
    {
        var todo = await repository.CreateAsync(request.Title);
        return Results.Created($"/api/todo/{todo.Id}", todo);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
})
.WithName("CreateTodo")
.WithOpenApi();

app.MapDelete("/api/todo/{id}", async (string id, ITodoRepository repository) =>
{
    var deleted = await repository.DeleteAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteTodo")
.WithOpenApi();

app.MapPatch("/api/todo/{id}/toggle", async (string id, ITodoRepository repository) =>
{
    var updatedTodo = await repository.ToggleCompleteAsync(id);
    return updatedTodo != null ? Results.Ok(updatedTodo) : Results.NotFound();
})
.WithName("ToggleTodoComplete")
.WithOpenApi();

app.Run();
