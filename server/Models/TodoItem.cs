namespace TodoApp.Server.Models;

public record TodoItem
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required bool Completed { get; init; }
    public DateTime? CompletedAt { get; init; }
}
