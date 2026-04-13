using TodoApp.BLL.Models;

namespace TodoApp.BLL.Interfaces;

public interface ITodoListService
{
    Task<TodoListResponseDto> CreateAsync(Guid userId, CreateTodoListDto dto);
    Task<IEnumerable<TodoListResponseDto>> GetAllAsync(Guid userId);
    Task<TodoListResponseDto?> GetByIdAsync(Guid userId, Guid listId);
    Task<TodoListResponseDto> UpdateAsync(Guid userId, Guid listId, UpdateTodoListDto dto);
    Task<bool> DeleteAsync(Guid userId, Guid listId);
}