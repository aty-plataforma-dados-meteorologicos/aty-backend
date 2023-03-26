using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Interfaces;
using AtyBackend.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AtyBackend.Infrastructure.Data.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : Entity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _entities;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _entities = context.Set<T>();
    }

    public async Task<T> CreateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        await _entities.AddAsync(entity);

        // log
        //var result = _entities.Add(entity);
        // System.Diagnostics.Trace.WriteLine(result.ToString());
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<int> CountAsync() => await _entities
        .Where(i => i.IsDeleted == false)
        .CountAsync();

    public async Task<int> CountByConditionAsync(Expression<Func<T, bool>> expression) => await _entities
        .Where(i => i.IsDeleted == false)
        .Where(expression)
        .CountAsync();

    public async Task<List<T>> GetAllAsync() => await _entities
        .Where(i => i.IsDeleted == false)
        .ToListAsync();

    public async Task<List<T>> GetAllAsync(int pageSize, int pageNumber) => await _entities
        .Where(i => i.IsDeleted == false)
        .OrderByDescending(i => i.Id)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    public async Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> expression) => await _entities
        .Where(i => i.IsDeleted == false)
        .Where(expression)
        .ToListAsync();

    public async Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> expression, int pageSize, int pageNumber) => await _entities
        .Where(i => i.IsDeleted == false)
        .Where(expression)
        .OrderByDescending(i => i.Id)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    public async Task<T> GetByIdAsync(int? id) => await _entities
        .Where(i => i.IsDeleted == false)
        .SingleOrDefaultAsync(s => s.Id == id);

    public async Task<T> UpdateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (await _entities.AnyAsync(i => i.Id == entity.Id && i.IsDeleted == false))
        {
            _entities.Update(entity);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new Exception("Entity not found");
        }

        return entity;
    }

    public async Task<bool> DeleteAsync(T entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (await _entities.AnyAsync(i => i.Id == entity.Id && i.IsDeleted == false))
        {
            entity.IsDeleted = true;
            _entities.Update(entity);
            var result = await _context.SaveChangesAsync();
            
            return (result > 0);
        }

        return false;
    }
}

