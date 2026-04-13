namespace TodoApp.BLL.Models;

public class CreateTodoListDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateTodoListDto
{
    public string Name { get; set; } = string.Empty;
}

public class TodoListResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int TodoItemsCount { get; set; }
}