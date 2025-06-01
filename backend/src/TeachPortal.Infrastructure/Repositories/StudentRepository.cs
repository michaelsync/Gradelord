using Microsoft.EntityFrameworkCore;
using TeachPortal.Domain.Entities;
using TeachPortal.Domain.Interfaces;
using TeachPortal.Infrastructure.Data;

namespace TeachPortal.Infrastructure.Repositories;

public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(TeachPortalDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Student>> GetStudentsByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.TeacherId == teacherId)
            .Include(s => s.Teacher)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(s => s.Email == email);
        
        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }
        
        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<Student?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Teacher)
            .FirstOrDefaultAsync(s => s.Email == email, cancellationToken);
    }

    public async Task<int> GetStudentCountByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(s => s.TeacherId == teacherId, cancellationToken);
    }
} 