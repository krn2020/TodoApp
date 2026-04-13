using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoApp.BLL.Interfaces;
using TodoApp.BLL.Models;
using Microsoft.AspNetCore.Authorization;

namespace TodoApp.Api.Endpoints;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/todos")
                       .WithTags("Todos")
                       .RequireAuthorization();

        var todosGroup = app.MapGroup("/api/todos")
                           .WithTags("Todos")
                           .RequireAuthorization();

        group.MapPost("/", async ([FromBody] CreateTodoDto dto, ITodoService todoService, ClaimsPrincipal user) =>
        {
            var userId = GetUserId(user);
            var todo = await todoService.CreateAsync(userId, dto);
            return Results.Created($"/api/todos/{todo.Id}", todo);
        })
        .WithName("CreateTodo")
        .Produces<TodoResponseDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/", async (
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            ITodoService todoService = null!,
            ClaimsPrincipal user = null!) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var userId = GetUserId(user);
            var result = await todoService.GetAllPaginatedAsync(userId, page, pageSize);
            return Results.Ok(result);
        })
        .WithName("GetAllTodos")
        .Produces<PagedResult<TodoResponseDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", async (Guid id, ITodoService todoService, ClaimsPrincipal user) =>
        {
            var userId = GetUserId(user);
            var todo = await todoService.GetByIdAsync(userId, id);
            return todo is null ? Results.NotFound() : Results.Ok(todo);
        })
        .WithName("GetTodoById")
        .Produces<TodoResponseDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateTodoDto dto, ITodoService todoService, ClaimsPrincipal user) =>
        {
            var userId = GetUserId(user);
            try
            {
                var updated = await todoService.UpdateAsync(userId, id, dto);
                return Results.Ok(updated);
            }
            catch (InvalidOperationException)
            {
                return Results.NotFound();
            }
        })
        .WithName("UpdateTodo")
        .Produces<TodoResponseDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (Guid id, ITodoService todoService, ClaimsPrincipal user) =>
        {
            var userId = GetUserId(user);
            var deleted = await todoService.DeleteAsync(userId, id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteTodo")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        todosGroup.MapPost("/{todoId:guid}/subtasks", async (Guid todoId, [FromBody] CreateSubTaskDto dto, ITodoService todoService, ClaimsPrincipal user) =>
        {
            var userId = GetUserId(user);
            try
            {
                var subTask = await todoService.AddSubTaskAsync(userId, todoId, dto);
                return Results.Created($"/api/todos/{todoId}/subtasks/{subTask.Id}", subTask);
            }
            catch (InvalidOperationException ex)
            {
                return Results.NotFound(ex.Message);
            }
        }).WithName("AddSubTask").Produces<SubTaskResponseDto>(201).Produces(404);

        todosGroup.MapPut("/subtasks/{subTaskId:guid}", async (Guid subTaskId, [FromBody] UpdateSubTaskDto dto, ITodoService todoService, ClaimsPrincipal user) =>
        {
            var userId = GetUserId(user);
            try
            {
                var updated = await todoService.UpdateSubTaskAsync(userId, subTaskId, dto);
                return Results.Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return Results.NotFound(ex.Message);
            }
        }).WithName("UpdateSubTask").Produces<SubTaskResponseDto>(200).Produces(404);

        todosGroup.MapDelete("/subtasks/{subTaskId:guid}", async (Guid subTaskId, ITodoService todoService, ClaimsPrincipal user) =>
        {
            var userId = GetUserId(user);
            var deleted = await todoService.DeleteSubTaskAsync(userId, subTaskId);
            return deleted ? Results.NoContent() : Results.NotFound();
        }).WithName("DeleteSubTask").Produces(204).Produces(404);

        var listsGroup = app.MapGroup("/api/lists")
                           .WithTags("TodoLists")
                           .RequireAuthorization();

        listsGroup.MapPost("/", async ([FromBody] CreateTodoListDto dto, ITodoListService listService, ClaimsPrincipal user) =>
        {
            var userId = GetUserId(user);
            var list = await listService.CreateAsync(userId, dto);
            return Results.Created($"/api/lists/{list.Id}", list);
        }).WithName("CreateList").Produces<TodoListResponseDto>(201);

        listsGroup.MapGet("/", async (ITodoListService listService, ClaimsPrincipal user) =>
        {
            var userId = GetUserId(user);
            var lists = await listService.GetAllAsync(userId);
            return Results.Ok(lists);
        }).WithName("GetAllLists").Produces<IEnumerable<TodoListResponseDto>>();

        listsGroup.MapGet("/{listId:guid}", async (Guid listId, ITodoListService listService, ClaimsPrincipal user) =>
        {
            var userId = GetUserId(user);
            var list = await listService.GetByIdAsync(userId, listId);
            return list is null ? Results.NotFound() : Results.Ok(list);
        }).WithName("GetListById").Produces<TodoListResponseDto>().Produces(404);

        listsGroup.MapPut("/{listId:guid}", async (Guid listId, [FromBody] UpdateTodoListDto dto, ITodoListService listService, ClaimsPrincipal user) =>
        {
            var userId = GetUserId(user);
            try
            {
                var updated = await listService.UpdateAsync(userId, listId, dto);
                return Results.Ok(updated);
            }
            catch (InvalidOperationException)
            {
                return Results.NotFound();
            }
        }).WithName("UpdateList").Produces<TodoListResponseDto>().Produces(404);

        listsGroup.MapDelete("/{listId:guid}", async (Guid listId, ITodoListService listService, ClaimsPrincipal user) =>
        {
            var userId = GetUserId(user);
            var deleted = await listService.DeleteAsync(userId, listId);
            return deleted ? Results.NoContent() : Results.NotFound();
        }).WithName("DeleteList").Produces(204).Produces(404);

        listsGroup.MapGet("/{listId:guid}/todos", async (Guid listId, [FromQuery] int page, [FromQuery] int pageSize, ITodoService todoService, ClaimsPrincipal user) =>
        {
            var userId = GetUserId(user);
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            var result = await todoService.GetAllPaginatedAsync(userId, page, pageSize, listId);
            return Results.Ok(result);
        }).WithName("GetTodosByList").Produces<PagedResult<TodoResponseDto>>();
    }

    private static Guid GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? throw new UnauthorizedAccessException("User ID not found in token.");
        return Guid.Parse(userIdClaim);
    }
}