using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TodoApp.BLL.Interfaces;
using TodoApp.BLL.Models;
using TodoApp.DAL.Entities;
using TodoApp.DAL.Interfaces;
using TodoApp.DAL.Repositories;
using BC = BCrypt.Net.BCrypt;

namespace TodoApp.BLL.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<UserResponseDto> RegisterAsync(RegisterUserDto registerDto)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(registerDto.Username);
        if (existingUser != null)
            throw new InvalidOperationException("Username already taken.");

        existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = BC.HashPassword(registerDto.Password)
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<string> LoginAsync(LoginUserDto loginDto)
    {
        var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
        if (user == null || !BC.Verify(loginDto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid username or password.");

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"]!));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}