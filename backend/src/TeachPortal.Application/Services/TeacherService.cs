using TeachPortal.Application.DTOs.Auth;
using TeachPortal.Domain.Interfaces;

namespace TeachPortal.Application.Services;

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teacherRepository;

    public TeacherService(ITeacherRepository teacherRepository)
    {
        _teacherRepository = teacherRepository;
    }

    public async Task<IEnumerable<TeacherDto>> GetAllTeachersAsync(CancellationToken cancellationToken = default)
    {
        var teachers = await _teacherRepository.GetAllWithStudentCountsAsync(cancellationToken);
        
        return teachers.Select(t => new TeacherDto
        {
            Id = t.Id,
            Username = t.Username,
            Email = t.Email,
            FirstName = t.FirstName,
            LastName = t.LastName,
            FullName = t.FullName,
            StudentCount = t.StudentCount,
            CreatedAt = t.CreatedAt
        });
    }

    public async Task<TeacherDto?> GetTeacherByIdAsync(Guid teacherId, CancellationToken cancellationToken = default)
    {
        var teacher = await _teacherRepository.GetByIdAsync(teacherId, cancellationToken);
        if (teacher == null)
        {
            return null;
        }

        return new TeacherDto
        {
            Id = teacher.Id,
            Username = teacher.Username,
            Email = teacher.Email,
            FirstName = teacher.FirstName,
            LastName = teacher.LastName,
            FullName = teacher.FullName,
            StudentCount = teacher.StudentCount,
            CreatedAt = teacher.CreatedAt
        };
    }

    public async Task<TeacherDto?> GetTeacherWithStudentsAsync(Guid teacherId, CancellationToken cancellationToken = default)
    {
        var teacher = await _teacherRepository.GetWithStudentsAsync(teacherId, cancellationToken);
        if (teacher == null)
        {
            return null;
        }

        return new TeacherDto
        {
            Id = teacher.Id,
            Username = teacher.Username,
            Email = teacher.Email,
            FirstName = teacher.FirstName,
            LastName = teacher.LastName,
            FullName = teacher.FullName,
            StudentCount = teacher.StudentCount,
            CreatedAt = teacher.CreatedAt
        };
    }
} 