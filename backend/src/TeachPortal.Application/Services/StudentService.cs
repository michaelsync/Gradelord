using TeachPortal.Application.DTOs.Students;
using TeachPortal.Domain.Entities;
using TeachPortal.Domain.Interfaces;

namespace TeachPortal.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly ITeacherRepository _teacherRepository;

    public StudentService(IStudentRepository studentRepository, ITeacherRepository teacherRepository)
    {
        _studentRepository = studentRepository;
        _teacherRepository = teacherRepository;
    }

    public async Task<StudentDto> CreateStudentAsync(CreateStudentRequestDto request, Guid teacherId, CancellationToken cancellationToken = default)
    {
        // Verify teacher exists
        var teacher = await _teacherRepository.GetByIdAsync(teacherId, cancellationToken);
        if (teacher == null)
        {
            throw new InvalidOperationException("Teacher not found");
        }

        // Check if email is unique
        if (!await _studentRepository.IsEmailUniqueAsync(request.Email, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("Email already exists");
        }

        var student = new Student
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            TeacherId = teacherId,
            CreatedBy = teacher.Username
        };

        await _studentRepository.AddAsync(student, cancellationToken);

        return new StudentDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            FullName = student.FullName,
            Email = student.Email,
            TeacherId = student.TeacherId,
            TeacherName = teacher.FullName,
            CreatedAt = student.CreatedAt
        };
    }

    public async Task<IEnumerable<StudentDto>> GetStudentsByTeacherAsync(Guid teacherId, CancellationToken cancellationToken = default)
    {
        var students = await _studentRepository.GetStudentsByTeacherIdAsync(teacherId, cancellationToken);
        
        return students.Select(s => new StudentDto
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            FullName = s.FullName,
            Email = s.Email,
            TeacherId = s.TeacherId,
            TeacherName = s.Teacher?.FullName ?? string.Empty,
            CreatedAt = s.CreatedAt
        });
    }

    public async Task<StudentDto?> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);
        if (student == null)
        {
            return null;
        }

        var teacher = await _teacherRepository.GetByIdAsync(student.TeacherId, cancellationToken);

        return new StudentDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            FullName = student.FullName,
            Email = student.Email,
            TeacherId = student.TeacherId,
            TeacherName = teacher?.FullName ?? string.Empty,
            CreatedAt = student.CreatedAt
        };
    }

    public async Task<StudentDto> UpdateStudentAsync(Guid studentId, CreateStudentRequestDto request, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);
        if (student == null)
        {
            throw new InvalidOperationException("Student not found");
        }

        // Check if email is unique (excluding current student)
        if (!await _studentRepository.IsEmailUniqueAsync(request.Email, studentId, cancellationToken))
        {
            throw new InvalidOperationException("Email already exists");
        }

        student.FirstName = request.FirstName;
        student.LastName = request.LastName;
        student.Email = request.Email;

        await _studentRepository.UpdateAsync(student, cancellationToken);

        var teacher = await _teacherRepository.GetByIdAsync(student.TeacherId, cancellationToken);

        return new StudentDto
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            FullName = student.FullName,
            Email = student.Email,
            TeacherId = student.TeacherId,
            TeacherName = teacher?.FullName ?? string.Empty,
            CreatedAt = student.CreatedAt
        };
    }

    public async Task DeleteStudentAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);
        if (student == null)
        {
            throw new InvalidOperationException("Student not found");
        }

        await _studentRepository.DeleteAsync(studentId, cancellationToken);
    }
} 