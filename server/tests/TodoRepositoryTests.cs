using FluentAssertions;
using TodoApp.Server.Models;
using TodoApp.Server.Services;

namespace TodoApp.Server.Tests;

public class TodoRepositoryTests
{
    private readonly ITodoRepository _repository;

    public TodoRepositoryTests()
    {
        _repository = new InMemoryTodoRepository();
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_WithValidTitle_ShouldCreateTodo()
    {
        // Arrange
        var title = "Test Todo";

        // Act
        var result = await _repository.CreateAsync(title);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(title);
        result.Id.Should().NotBeNullOrEmpty();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.Completed.Should().BeFalse();
        result.CompletedAt.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateAsync_WithInvalidTitle_ShouldThrowArgumentException(string? title)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.CreateAsync(title!));
    }

    [Fact]
    public async Task CreateAsync_ShouldTrimTitle()
    {
        // Arrange
        var titleWithSpaces = "  Test Todo  ";
        var expectedTitle = "Test Todo";

        // Act
        var result = await _repository.CreateAsync(titleWithSpaces);

        // Assert
        result.Title.Should().Be(expectedTitle);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldReturnTrue()
    {
        // Arrange
        var todo = await _repository.CreateAsync("Test Todo");

        // Act
        var result = await _repository.DeleteAsync(todo.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ShouldReturnFalse()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid().ToString();

        // Act
        var result = await _repository.DeleteAsync(nonExistingId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ToggleCompleteAsync_WithExistingId_ShouldToggleCompletion()
    {
        // Arrange
        var todo = await _repository.CreateAsync("Test Todo");
        todo.Completed.Should().BeFalse();

        // Act
        var result = await _repository.ToggleCompleteAsync(todo.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Completed.Should().BeTrue();
        result.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ToggleCompleteAsync_WithCompletedTodo_ShouldUncomplete()
    {
        // Arrange
        var todo = await _repository.CreateAsync("Test Todo");
        var completedTodo = await _repository.ToggleCompleteAsync(todo.Id);
        completedTodo!.Completed.Should().BeTrue();

        // Act
        var result = await _repository.ToggleCompleteAsync(todo.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Completed.Should().BeFalse();
        result.CompletedAt.Should().BeNull();
    }

    [Fact]
    public async Task ToggleCompleteAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid().ToString();

        // Act
        var result = await _repository.ToggleCompleteAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnTodosInDescendingOrder()
    {
        // Arrange
        var firstTodo = await _repository.CreateAsync("First Todo");
        await Task.Delay(100); // Ensure different timestamps
        var secondTodo = await _repository.CreateAsync("Second Todo");

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Id.Should().Be(secondTodo.Id); // Most recent first
        result.Last().Id.Should().Be(firstTodo.Id);
    }
}
