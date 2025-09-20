using System.ComponentModel.DataAnnotations;

namespace TodoApp.Server.Models;

public record CreateTodoRequest
{
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public required string Title { get; init; }
}
