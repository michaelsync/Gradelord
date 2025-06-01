using Microsoft.EntityFrameworkCore;
using TeachPortal.Domain.Entities;
using TeachPortal.Domain.Interfaces;
using TeachPortal.Infrastructure.Data;

namespace TeachPortal.Infrastructure.Repositories;

public class TeacherRepository : Repository<Teacher>, ITeacherRepository
{
    public TeacherRepository(TeachPortalDbContext context) : base(context)
    {
    }

    public async Task<Teacher?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Username == username, cancellationToken);
    }

    public async Task<Teacher?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Email == email, cancellationToken);
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(t => t.Username == username);

        if (excludeId.HasValue)
        {
            query = query.Where(t => t.Id != excludeId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(t => t.Email == email);

        if (excludeId.HasValue)
        {
            query = query.Where(t => t.Id != excludeId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<Teacher>> GetAllWithStudentCountsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Students)
            .ToListAsync(cancellationToken);
    }
}