using TeachPortal.Domain.Entities;

namespace TeachPortal.Domain.Interfaces;

public interface ITeacherRepository : IRepository<Teacher>
{
    Task<Teacher?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<Teacher?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Teacher>> GetAllWithStudentCountsAsync(CancellationToken cancellationToken = default);
}