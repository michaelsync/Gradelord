using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TeachPortal.Application.DTOs.Students;
using TeachPortal.Application.Services;
using TeachPortal.Domain.Entities;
using TeachPortal.Domain.Interfaces;
using Xunit;

namespace TeachPortal.Tests.Unit.Services;

public class StudentServiceTests
{
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly Mock<ITeacherRepository> _teacherRepositoryMock;
    private readonly StudentService _studentService;

    public StudentServiceTests()
    {
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _teacherRepositoryMock = new Mock<ITeacherRepository>();
        _studentService = new StudentService(
            _studentRepositoryMock.Object,
            _teacherRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateStudentAsync_WithValidData_ShouldReturnStudentDto()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var createRequest = new CreateStudentRequestDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        var teacher = new Teacher
        {
            Id = teacherId,
            Username = "teacher1",
            FirstName = "Teacher",
            LastName = "One"
        };

        var savedStudent = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = createRequest.FirstName,
            LastName = createRequest.LastName,
            Email = createRequest.Email,
            TeacherId = teacherId,
            Teacher = teacher
        };

        _teacherRepositoryMock
            .Setup(x => x.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teacher);

        _studentRepositoryMock
            .Setup(x => x.IsEmailUniqueAsync(createRequest.Email, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _studentRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(savedStudent);

        // Act
        var result = await _studentService.CreateStudentAsync(createRequest, teacherId);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be(createRequest.FirstName);
        result.LastName.Should().Be(createRequest.LastName);
        result.Email.Should().Be(createRequest.Email);
        result.TeacherId.Should().Be(teacherId);
    }

    [Fact]
    public async Task GetStudentsByTeacherAsync_ShouldReturnStudentList()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var students = new List<Student>
        {
            new Student
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                TeacherId = teacherId,
                Teacher = new Teacher { FirstName = "Teacher", LastName = "One" }
            },
            new Student
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                TeacherId = teacherId,
                Teacher = new Teacher { FirstName = "Teacher", LastName = "One" }
            }
        };

        _studentRepositoryMock
            .Setup(x => x.GetStudentsByTeacherIdAsync(teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(students);

        // Act
        var result = await _studentService.GetStudentsByTeacherAsync(teacherId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().FirstName.Should().Be("John");
        result.Last().FirstName.Should().Be("Jane");
    }

    [Fact]
    public async Task GetStudentByIdAsync_WithExistingStudent_ShouldReturnStudentDto()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var teacherId = Guid.NewGuid();
        var student = new Student
        {
            Id = studentId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            TeacherId = teacherId
        };

        var teacher = new Teacher
        {
            Id = teacherId,
            FirstName = "Teacher",
            LastName = "One"
        };

        _studentRepositoryMock
            .Setup(x => x.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        _teacherRepositoryMock
            .Setup(x => x.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teacher);

        // Act
        var result = await _studentService.GetStudentByIdAsync(studentId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(studentId);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task GetStudentByIdAsync_WithNonExistingStudent_ShouldReturnNull()
    {
        // Arrange
        var studentId = Guid.NewGuid();

        _studentRepositoryMock
            .Setup(x => x.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);

        // Act
        var result = await _studentService.GetStudentByIdAsync(studentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateStudentAsync_WithValidData_ShouldReturnUpdatedStudentDto()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var teacherId = Guid.NewGuid();
        var updateRequest = new CreateStudentRequestDto
        {
            FirstName = "Updated",
            LastName = "Name",
            Email = "updated@example.com"
        };

        var existingStudent = new Student
        {
            Id = studentId,
            FirstName = "Old",
            LastName = "Name",
            Email = "old@example.com",
            TeacherId = teacherId
        };

        var teacher = new Teacher
        {
            Id = teacherId,
            FirstName = "Teacher",
            LastName = "One"
        };

        _studentRepositoryMock
            .Setup(x => x.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingStudent);

        _studentRepositoryMock
            .Setup(x => x.IsEmailUniqueAsync(updateRequest.Email, studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _studentRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Student>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student student, CancellationToken ct) => student);

        _teacherRepositoryMock
            .Setup(x => x.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teacher);

        // Act
        var result = await _studentService.UpdateStudentAsync(studentId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be(updateRequest.FirstName);
        result.LastName.Should().Be(updateRequest.LastName);
        result.Email.Should().Be(updateRequest.Email);
    }

    [Fact]
    public async Task DeleteStudentAsync_WithExistingStudent_ShouldCallRepositoryDelete()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var student = new Student
        {
            Id = studentId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };

        _studentRepositoryMock
            .Setup(x => x.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        // Act
        await _studentService.DeleteStudentAsync(studentId);

        // Assert
        _studentRepositoryMock.Verify(
            x => x.DeleteAsync(studentId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}