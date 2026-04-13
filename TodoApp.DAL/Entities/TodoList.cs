namespace TodoApp.DAL.Entities;

public class TodoList
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
}