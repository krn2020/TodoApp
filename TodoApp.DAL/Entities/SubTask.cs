namespace TodoApp.DAL.Entities;

public class SubTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid TodoItemId { get; set; }
    public TodoItem TodoItem { get; set; } = null!;
}