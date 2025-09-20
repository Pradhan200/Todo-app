using TodoApp.Server.Models;

namespace TodoApp.Server.Services;

public interface ITodoRepository
{
    Task<IEnumerable<TodoItem>> GetAllAsync();
    Task<TodoItem> CreateAsync(string title);
    Task<bool> DeleteAsync(string id);
    Task<TodoItem?> ToggleCompleteAsync(string id);
}
