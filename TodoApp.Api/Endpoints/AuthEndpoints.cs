using Microsoft.AspNetCore.Mvc;
using TodoApp.BLL.Interfaces;
using TodoApp.BLL.Models;

namespace TodoApp.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/register", async ([FromBody] RegisterUserDto dto, IAuthService authService) =>
        {
            try
            {
                var user = await authService.RegisterAsync(dto);
                return Results.Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithName("Register")
        .Produces<UserResponseDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/login", async ([FromBody] LoginUserDto dto, IAuthService authService) =>
        {
            try
            {
                var token = await authService.LoginAsync(dto);
                return Results.Ok(new { token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Unauthorized();
            }
        })
        .WithName("Login")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}