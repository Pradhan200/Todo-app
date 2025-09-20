using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoApp.Server.Models;

namespace TodoApp.Server.Tests;

public class TodoApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TodoApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetTodos_ReturnsEmptyList_WhenNoTodosExist()
    {
        // Act
        var response = await _client.GetAsync("/api/todo");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<TodoItem[]>();
        todos.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateTodo_WithValidRequest_ReturnsCreatedTodo()
    {
        // Arrange
        var request = new CreateTodoRequest { Title = "Integration Test Todo" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/todo", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdTodo = await response.Content.ReadFromJsonAsync<TodoItem>();
        createdTodo.Should().NotBeNull();
        createdTodo!.Title.Should().Be("Integration Test Todo");
        createdTodo.Id.Should().NotBeNullOrEmpty();
        createdTodo.Completed.Should().BeFalse();
        createdTodo.CompletedAt.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateTodo_WithInvalidTitle_ReturnsBadRequest(string? title)
    {
        // Arrange
        var request = new CreateTodoRequest { Title = title! };

        // Act
        var response = await _client.PostAsJsonAsync("/api/todo", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ToggleTodoComplete_WithValidId_ReturnsUpdatedTodo()
    {
        // Arrange
        var todo = await CreateTodoAsync("Toggle Test Todo");

        // Act
        var response = await _client.PatchAsync($"/api/todo/{todo.Id}/toggle", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedTodo = await response.Content.ReadFromJsonAsync<TodoItem>();
        updatedTodo.Should().NotBeNull();
        updatedTodo!.Completed.Should().BeTrue();
        updatedTodo.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ToggleTodoComplete_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid().ToString();

        // Act
        var response = await _client.PatchAsync($"/api/todo/{nonExistingId}/toggle", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTodo_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var todo = await CreateTodoAsync("Delete Test Todo");

        // Act
        var response = await _client.DeleteAsync($"/api/todo/{todo.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify todo is deleted
        var getResponse = await _client.GetAsync("/api/todo");
        var todos = await getResponse.Content.ReadFromJsonAsync<TodoItem[]>();
        todos.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteTodo_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid().ToString();

        // Act
        var response = await _client.DeleteAsync($"/api/todo/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CompleteWorkflow_ShouldWorkEndToEnd()
    {
        // Arrange & Act
        // 1. Create a todo
        var createRequest = new CreateTodoRequest { Title = "Workflow Test Todo" };
        var createResponse = await _client.PostAsJsonAsync("/api/todo", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var todo = await createResponse.Content.ReadFromJsonAsync<TodoItem>();

        // 2. Verify it's in the list
        var getResponse = await _client.GetAsync("/api/todo");
        var todos = await getResponse.Content.ReadFromJsonAsync<TodoItem[]>();
        todos.Should().HaveCount(1);
        todos![0].Completed.Should().BeFalse();

        // 3. Complete the todo
        var toggleResponse = await _client.PatchAsync($"/api/todo/{todo!.Id}/toggle", null);
        toggleResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var completedTodo = await toggleResponse.Content.ReadFromJsonAsync<TodoItem>();
        completedTodo!.Completed.Should().BeTrue();

        // 4. Delete the todo
        var deleteResponse = await _client.DeleteAsync($"/api/todo/{todo.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 5. Verify it's gone
        var finalGetResponse = await _client.GetAsync("/api/todo");
        var finalTodos = await finalGetResponse.Content.ReadFromJsonAsync<TodoItem[]>();
        finalTodos.Should().BeEmpty();
    }

    private async Task<TodoItem> CreateTodoAsync(string title)
    {
        var request = new CreateTodoRequest { Title = title };
        var response = await _client.PostAsJsonAsync("/api/todo", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<TodoItem>())!;
    }
}
