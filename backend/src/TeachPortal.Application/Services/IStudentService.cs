using TeachPortal.Application.DTOs.Students;

namespace TeachPortal.Application.Services;

public interface IStudentService
{
    Task<StudentDto> CreateStudentAsync(CreateStudentRequestDto request, Guid teacherId, CancellationToken cancellationToken = default);
    Task<IEnumerable<StudentDto>> GetStudentsByTeacherAsync(Guid teacherId, CancellationToken cancellationToken = default);
    Task<StudentDto?> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<StudentDto> UpdateStudentAsync(Guid studentId, CreateStudentRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteStudentAsync(Guid studentId, CancellationToken cancellationToken = default);
} 