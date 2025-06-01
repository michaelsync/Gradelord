using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TeachPortal.Application.DTOs.Auth;
using TeachPortal.Domain.Entities;
using TeachPortal.Domain.Interfaces;
using BCrypt.Net;

namespace TeachPortal.Application.Services;

public class AuthService : IAuthService
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly IConfiguration _configuration;

    public AuthService(ITeacherRepository teacherRepository, IConfiguration configuration)
    {
        _teacherRepository = teacherRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var teacher = await _teacherRepository.GetByUsernameAsync(request.Username, cancellationToken);

        if (teacher == null || !BCrypt.Net.BCrypt.Verify(request.Password, teacher.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        if (!teacher.IsActive)
        {
            throw new UnauthorizedAccessException("Account is deactivated");
        }

        var token = GenerateJwtToken(teacher);
        var refreshToken = GenerateRefreshToken();

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            Expires = DateTime.UtcNow.AddMinutes(GetJwtExpiryMinutes()),
            Teacher = new TeacherDto
            {
                Id = teacher.Id,
                Username = teacher.Username,
                Email = teacher.Email,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                FullName = teacher.FullName,
                StudentCount = teacher.StudentCount,
                CreatedAt = teacher.CreatedAt
            }
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        // Check if username is unique
        if (!await _teacherRepository.IsUsernameUniqueAsync(request.Username, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("Username already exists");
        }

        // Check if email is unique
        if (!await _teacherRepository.IsEmailUniqueAsync(request.Email, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("Email already exists");
        }

        // Create new teacher
        var teacher = new Teacher
        {
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsActive = true,
            CreatedBy = request.Username
        };

        await _teacherRepository.AddAsync(teacher, cancellationToken);

        var token = GenerateJwtToken(teacher);
        var refreshToken = GenerateRefreshToken();

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            Expires = DateTime.UtcNow.AddMinutes(GetJwtExpiryMinutes()),
            Teacher = new TeacherDto
            {
                Id = teacher.Id,
                Username = teacher.Username,
                Email = teacher.Email,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                FullName = teacher.FullName,
                StudentCount = 0,
                CreatedAt = teacher.CreatedAt
            }
        };
    }

    public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(GetJwtSecretKey());

        var result = await tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = GetJwtIssuer(),
            ValidateAudience = true,
            ValidAudience = GetJwtAudience(),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        });

        return result.IsValid;

    }

    private string GenerateJwtToken(Teacher teacher)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(GetJwtSecretKey());

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, teacher.Id.ToString()),
                new Claim(ClaimTypes.Name, teacher.Username),
                new Claim(ClaimTypes.Email, teacher.Email),
                new Claim("firstName", teacher.FirstName),
                new Claim("lastName", teacher.LastName)
            }),
            Expires = DateTime.UtcNow.AddMinutes(GetJwtExpiryMinutes()),
            Issuer = GetJwtIssuer(),
            Audience = GetJwtAudience(),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }

    private string GetJwtSecretKey()
    {
        return _configuration["JWT:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
    }

    private string GetJwtIssuer()
    {
        return _configuration["JWT:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
    }

    private string GetJwtAudience()
    {
        return _configuration["JWT:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");
    }

    private int GetJwtExpiryMinutes()
    {
        return int.Parse(_configuration["JWT:ExpiryInMinutes"] ?? "60");
    }
}