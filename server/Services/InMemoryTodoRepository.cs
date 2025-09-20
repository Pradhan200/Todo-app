using System.Collections.Concurrent;
using TodoApp.Server.Models;

namespace TodoApp.Server.Services;

public class InMemoryTodoRepository : ITodoRepository
{
    private readonly ConcurrentDictionary<string, TodoItem> _todos = new();

    public Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        var todos = _todos.Values
            .OrderByDescending(t => t.CreatedAt)
            .AsEnumerable();
        
        return Task.FromResult(todos);
    }

    public Task<TodoItem> CreateAsync(string title)
    {
        var todo = new TodoItem
        {
            Id = Guid.NewGuid().ToString(),
            Title = title.Trim(),
            CreatedAt = DateTime.UtcNow,
            Completed = false
        };

        _todos.TryAdd(todo.Id, todo);
        return Task.FromResult(todo);
    }

    public Task<bool> DeleteAsync(string id)
    {
        return Task.FromResult(_todos.TryRemove(id, out _));
    }

    public Task<TodoItem?> ToggleCompleteAsync(string id)
    {
        if (_todos.TryGetValue(id, out var existingTodo))
        {
            var updatedTodo = existingTodo with
            {
                Completed = !existingTodo.Completed,
                CompletedAt = !existingTodo.Completed ? DateTime.UtcNow : null
            };

            _todos.TryUpdate(id, updatedTodo, existingTodo);
            return Task.FromResult<TodoItem?>(updatedTodo);
        }

        return Task.FromResult<TodoItem?>(null);
    }
}
