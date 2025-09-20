using FluentValidation;
using TodoApp.Server.Models;

namespace TodoApp.Server.Validators;

public class CreateTodoRequestValidator : AbstractValidator<CreateTodoRequest>
{
    public CreateTodoRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(500)
            .WithMessage("Title cannot exceed 500 characters")
            .MinimumLength(1)
            .WithMessage("Title must be at least 1 character")
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("Title cannot be only whitespace")
            .Must(title => !title.StartsWith(" ") && !title.EndsWith(" "))
            .WithMessage("Title cannot start or end with spaces");
    }
}
