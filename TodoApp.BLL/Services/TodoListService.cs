using TodoApp.BLL.Interfaces;
using TodoApp.BLL.Models;
using TodoApp.DAL.Entities;
using TodoApp.DAL.Interfaces;

namespace TodoApp.BLL.Services;

public class TodoListService : ITodoListService
{
    private readonly IRepository<TodoList> _listRepository;

    public TodoListService(IRepository<TodoList> listRepository)
    {
        _listRepository = listRepository;
    }

    public async Task<TodoListResponseDto> CreateAsync(Guid userId, CreateTodoListDto dto)
    {
        var list = new TodoList
        {
            Name = dto.Name,
            UserId = userId
        };
        await _listRepository.AddAsync(list);
        await _listRepository.SaveChangesAsync();
        return MapToDto(list);
    }

    public async Task<IEnumerable<TodoListResponseDto>> GetAllAsync(Guid userId)
    {
        var lists = await _listRepository.FindAsync(l => l.UserId == userId);
        return lists.Select(MapToDto);
    }

    public async Task<TodoListResponseDto?> GetByIdAsync(Guid userId, Guid listId)
    {
        var list = await _listRepository.GetByIdAsync(listId);
        if (list == null || list.UserId != userId) return null;
        return MapToDto(list);
    }

    public async Task<TodoListResponseDto> UpdateAsync(Guid userId, Guid listId, UpdateTodoListDto dto)
    {
        var list = await _listRepository.GetByIdAsync(listId);
        if (list == null || list.UserId != userId)
            throw new InvalidOperationException("List not found or access denied.");
        list.Name = dto.Name;
        _listRepository.Update(list);
        await _listRepository.SaveChangesAsync();
        return MapToDto(list);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid listId)
    {
        var list = await _listRepository.GetByIdAsync(listId);
        if (list == null || list.UserId != userId) return false;
        _listRepository.Delete(list);
        await _listRepository.SaveChangesAsync();
        return true;
    }

    private TodoListResponseDto MapToDto(TodoList list) => new()
    {
        Id = list.Id,
        Name = list.Name,
        CreatedAt = list.CreatedAt,
        TodoItemsCount = list.TodoItems?.Count ?? 0
    };
}