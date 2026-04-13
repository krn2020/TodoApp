using TodoApp.BLL.Models;

namespace TodoApp.BLL.Interfaces;

public interface ITodoService
{
    Task<TodoResponseDto> CreateAsync(Guid userId, CreateTodoDto dto);
    Task<TodoResponseDto?> GetByIdAsync(Guid userId, Guid todoId);
    Task<PagedResult<TodoResponseDto>> GetAllPaginatedAsync(Guid userId, int page, int pageSize, Guid? listId = null);
    Task<TodoResponseDto> UpdateAsync(Guid userId, Guid todoId, UpdateTodoDto dto);
    Task<bool> DeleteAsync(Guid userId, Guid todoId);

    Task<SubTaskResponseDto> AddSubTaskAsync(Guid userId, Guid todoId, CreateSubTaskDto dto);
    Task<SubTaskResponseDto> UpdateSubTaskAsync(Guid userId, Guid subTaskId, UpdateSubTaskDto dto);
    Task<bool> DeleteSubTaskAsync(Guid userId, Guid subTaskId);
}