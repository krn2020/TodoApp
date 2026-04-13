namespace TodoApp.DAL.Entities;

public class TodoItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ReminderDate { get; set; }

    public RecurrenceType RecurrenceType { get; set; } = RecurrenceType.None;
    public DateTime? RecurrenceEndDate { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? TodoListId { get; set; }
    public TodoList? TodoList { get; set; }

    public ICollection<SubTask> SubTasks { get; set; } = new List<SubTask>();
}

