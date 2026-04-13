using TodoApp.BLL.Interfaces;
using TodoApp.BLL.Models;
using TodoApp.DAL.Entities;
using TodoApp.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TodoApp.BLL.Services;

public class TodoService : ITodoService
{
    private readonly IRepository<TodoItem> _todoRepository;
    private readonly IRepository<SubTask> _subTaskRepository;

    public TodoService(IRepository<TodoItem> todoRepository, IRepository<SubTask> subTaskRepository)
    {
        _todoRepository = todoRepository;
        _subTaskRepository = subTaskRepository;
    }

    public async Task<TodoResponseDto> CreateAsync(Guid userId, CreateTodoDto dto)
    {
        var todo = new TodoItem
        {
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            ReminderDate = dto.ReminderDate,
            RecurrenceType = dto.RecurrenceType,
            RecurrenceEndDate = dto.RecurrenceEndDate,
            TodoListId = dto.TodoListId,
            UserId = userId
        };

        await _todoRepository.AddAsync(todo);
        await _todoRepository.SaveChangesAsync();
        return MapToDto(todo);
    }

    public async Task<TodoResponseDto?> GetByIdAsync(Guid userId, Guid todoId)
    {
        var todo = await _todoRepository.GetByIdAsync(todoId);
        if (todo == null || todo.UserId != userId) return null;

        var subTasks = await _subTaskRepository.FindAsync(s => s.TodoItemId == todoId);
        todo.SubTasks = subTasks.ToList();

        return MapToDto(todo);
    }

    public async Task<PagedResult<TodoResponseDto>> GetAllPaginatedAsync(Guid userId, int page, int pageSize, Guid? listId = null)
    {
        var filter = listId.HasValue
            ? (System.Linq.Expressions.Expression<Func<TodoItem, bool>>)(t => t.UserId == userId && t.TodoListId == listId)
            : t => t.UserId == userId;

        var (items, totalCount) = await _todoRepository.GetPaginatedAsync(
            page, pageSize,
            filter: filter,
            orderBy: q => q.OrderByDescending(t => t.CreatedAt)
        );

        foreach (var item in items)
        {
            var subTasks = await _subTaskRepository.FindAsync(s => s.TodoItemId == item.Id);
            item.SubTasks = subTasks.ToList();
        }

        return new PagedResult<TodoResponseDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<TodoResponseDto> UpdateAsync(Guid userId, Guid todoId, UpdateTodoDto dto)
    {
        var todo = await _todoRepository.GetByIdAsync(todoId);
        if (todo == null || todo.UserId != userId)
            throw new InvalidOperationException("Todo not found or access denied.");

        todo.Title = dto.Title;
        todo.Description = dto.Description;
        todo.IsCompleted = dto.IsCompleted;
        todo.DueDate = dto.DueDate;
        todo.ReminderDate = dto.ReminderDate;
        todo.RecurrenceType = dto.RecurrenceType;
        todo.RecurrenceEndDate = dto.RecurrenceEndDate;
        todo.TodoListId = dto.TodoListId;
        todo.UpdatedAt = DateTime.UtcNow;

        _todoRepository.Update(todo);
        await _todoRepository.SaveChangesAsync();

        var subTasks = await _subTaskRepository.FindAsync(s => s.TodoItemId == todoId);
        todo.SubTasks = subTasks.ToList();
        return MapToDto(todo);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid todoId)
    {
        var todo = await _todoRepository.GetByIdAsync(todoId);
        if (todo == null || todo.UserId != userId) return false;

        _todoRepository.Delete(todo);
        await _todoRepository.SaveChangesAsync();
        return true;
    }

    public async Task<SubTaskResponseDto> AddSubTaskAsync(Guid userId, Guid todoId, CreateSubTaskDto dto)
    {
        var todo = await _todoRepository.GetByIdAsync(todoId);
        if (todo == null || todo.UserId != userId)
            throw new InvalidOperationException("Todo not found or access denied.");

        var subTask = new SubTask
        {
            Title = dto.Title,
            TodoItemId = todoId
        };
        await _subTaskRepository.AddAsync(subTask);
        await _subTaskRepository.SaveChangesAsync();
        return new SubTaskResponseDto
        {
            Id = subTask.Id,
            Title = subTask.Title,
            IsCompleted = subTask.IsCompleted,
            CreatedAt = subTask.CreatedAt
        };
    }

    public async Task<SubTaskResponseDto> UpdateSubTaskAsync(Guid userId, Guid subTaskId, UpdateSubTaskDto dto)
    {
        var subTask = await _subTaskRepository.GetByIdAsync(subTaskId);
        if (subTask == null) throw new InvalidOperationException("SubTask not found.");

        var todo = await _todoRepository.GetByIdAsync(subTask.TodoItemId);
        if (todo == null || todo.UserId != userId)
            throw new InvalidOperationException("Access denied.");

        subTask.Title = dto.Title;
        subTask.IsCompleted = dto.IsCompleted;
        _subTaskRepository.Update(subTask);
        await _subTaskRepository.SaveChangesAsync();

        return new SubTaskResponseDto
        {
            Id = subTask.Id,
            Title = subTask.Title,
            IsCompleted = subTask.IsCompleted,
            CreatedAt = subTask.CreatedAt
        };
    }

    public async Task<bool> DeleteSubTaskAsync(Guid userId, Guid subTaskId)
    {
        var subTask = await _subTaskRepository.GetByIdAsync(subTaskId);
        if (subTask == null) return false;

        var todo = await _todoRepository.GetByIdAsync(subTask.TodoItemId);
        if (todo == null || todo.UserId != userId) return false;

        _subTaskRepository.Delete(subTask);
        await _subTaskRepository.SaveChangesAsync();
        return true;
    }

    private TodoResponseDto MapToDto(TodoItem todo) => new()
    {
        Id = todo.Id,
        Title = todo.Title,
        Description = todo.Description,
        IsCompleted = todo.IsCompleted,
        CreatedAt = todo.CreatedAt,
        UpdatedAt = todo.UpdatedAt,
        DueDate = todo.DueDate,
        ReminderDate = todo.ReminderDate,
        RecurrenceType = todo.RecurrenceType,
        RecurrenceEndDate = todo.RecurrenceEndDate,
        UserId = todo.UserId,
        TodoListId = todo.TodoListId,
        SubTasks = todo.SubTasks.Select(s => new SubTaskResponseDto
        {
            Id = s.Id,
            Title = s.Title,
            IsCompleted = s.IsCompleted,
            CreatedAt = s.CreatedAt
        }).ToList()
    };
}