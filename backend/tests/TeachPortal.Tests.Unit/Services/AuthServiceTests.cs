using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TeachPortal.Application.DTOs.Auth;
using TeachPortal.Application.Services;
using TeachPortal.Domain.Entities;
using TeachPortal.Domain.Interfaces;
using Xunit;

namespace TeachPortal.Tests.Unit.Services;

public class AuthServiceTests
{
    private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _teacherRepositoryMock = new Mock<ITeacherRepository>();
        _configurationMock = new Mock<IConfiguration>();

        // Setup configuration mocks
        _configurationMock.Setup(x => x["JWT:SecretKey"]).Returns("SuperSecretKeyForTeachPortalApplication2024!");
        _configurationMock.Setup(x => x["JWT:Issuer"]).Returns("TeachPortal");
        _configurationMock.Setup(x => x["JWT:Audience"]).Returns("TeachPortalUsers");
        _configurationMock.Setup(x => x["JWT:ExpiryInMinutes"]).Returns("60");

        _authService = new AuthService(_teacherRepositoryMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnAuthResponse()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "testuser",
            Password = "password123"
        };

        var teacher = new Teacher
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            IsActive = true
        };

        _teacherRepositoryMock
            .Setup(x => x.GetByUsernameAsync(loginRequest.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teacher);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.Teacher.Username.Should().Be(teacher.Username);
        result.Teacher.Email.Should().Be(teacher.Email);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidUsername_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "nonexistent",
            Password = "password123"
        };

        _teacherRepositoryMock
            .Setup(x => x.GetByUsernameAsync(loginRequest.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Teacher?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(loginRequest));
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "testuser",
            Password = "wrongpassword"
        };

        var teacher = new Teacher
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
            IsActive = true
        };

        _teacherRepositoryMock
            .Setup(x => x.GetByUsernameAsync(loginRequest.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teacher);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(loginRequest));
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldReturnAuthResponse()
    {
        // Arrange
        var registerRequest = new RegisterRequestDto
        {
            Username = "newuser",
            Email = "newuser@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        var savedTeacher = new Teacher
        {
            Id = Guid.NewGuid(),
            Username = registerRequest.Username,
            Email = registerRequest.Email,
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password),
            IsActive = true
        };

        _teacherRepositoryMock
            .Setup(x => x.IsUsernameUniqueAsync(registerRequest.Username, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _teacherRepositoryMock
            .Setup(x => x.IsEmailUniqueAsync(registerRequest.Email, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _teacherRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Teacher>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(savedTeacher);

        // Act
        var result = await _authService.RegisterAsync(registerRequest);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.Teacher.Username.Should().Be(registerRequest.Username);
        result.Teacher.Email.Should().Be(registerRequest.Email);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateUsername_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var registerRequest = new RegisterRequestDto
        {
            Username = "existinguser",
            Email = "newuser@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        _teacherRepositoryMock
            .Setup(x => x.IsUsernameUniqueAsync(registerRequest.Username, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.RegisterAsync(registerRequest));
    }

    [Fact]
    public async Task ValidateTokenAsync_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        var teacher = new Teacher
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            IsActive = true
        };

        // Generate a valid token using the service
        var loginRequest = new LoginRequestDto { Username = "testuser", Password = "password123" };
        teacher.PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123");

        _teacherRepositoryMock
            .Setup(x => x.GetByUsernameAsync(loginRequest.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teacher);

        var authResponse = await _authService.LoginAsync(loginRequest);

        // Act
        var result = await _authService.ValidateTokenAsync(authResponse.Token);

        // Assert
        result.Should().BeTrue();
    }
}