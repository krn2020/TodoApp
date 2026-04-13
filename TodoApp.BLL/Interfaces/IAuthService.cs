using TodoApp.BLL.Models;

namespace TodoApp.BLL.Interfaces;

public interface IAuthService
{
    Task<UserResponseDto> RegisterAsync(RegisterUserDto registerDto);
    Task<string> LoginAsync(LoginUserDto loginDto);
}