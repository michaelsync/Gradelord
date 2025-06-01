using TeachPortal.Domain.Entities;

namespace TeachPortal.Domain.Interfaces;

public interface IStudentRepository : IRepository<Student>
{
    Task<IEnumerable<Student>> GetStudentsByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<Student?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<int> GetStudentCountByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default);
} 