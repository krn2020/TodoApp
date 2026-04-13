namespace TodoApp.BLL.Models;

public class CreateTodoDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ReminderDate { get; set; }
    public RecurrenceType RecurrenceType { get; set; } = RecurrenceType.None;
    public DateTime? RecurrenceEndDate { get; set; }
    public Guid? TodoListId { get; set; }
}

public class UpdateTodoDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ReminderDate { get; set; }
    public RecurrenceType RecurrenceType { get; set; }
    public DateTime? RecurrenceEndDate { get; set; }
    public Guid? TodoListId { get; set; }
}

public class TodoResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ReminderDate { get; set; }
    public RecurrenceType RecurrenceType { get; set; }
    public DateTime? RecurrenceEndDate { get; set; }
    public Guid UserId { get; set; }
    public Guid? TodoListId { get; set; }
    public List<SubTaskResponseDto> SubTasks { get; set; } = new();
}