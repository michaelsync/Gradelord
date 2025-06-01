using TeachPortal.Application.DTOs.Auth;

namespace TeachPortal.Application.Services;

public interface ITeacherService
{
    Task<IEnumerable<TeacherDto>> GetAllTeachersAsync(CancellationToken cancellationToken = default);
    Task<TeacherDto?> GetTeacherByIdAsync(Guid teacherId, CancellationToken cancellationToken = default);
}