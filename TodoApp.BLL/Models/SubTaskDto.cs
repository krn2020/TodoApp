namespace TodoApp.BLL.Models;

public class CreateSubTaskDto
{
    public string Title { get; set; } = string.Empty;
}

public class UpdateSubTaskDto
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}

public class SubTaskResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}