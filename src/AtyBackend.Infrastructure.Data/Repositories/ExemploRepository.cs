using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Interfaces;
using AtyBackend.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace AtyBackend.Infrastructure.Data.Repositories
{
    public class ExemploRepository : IExemploRepository
    {
        private ApplicationDbContext _context;
        private readonly DbSet<Exemplo> _entitiesExemplo;

        public ExemploRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _entitiesExemplo = context.Set<Exemplo>();
        }

        public async Task<Exemplo> CreateAsync(Exemplo entity)
        {
            var transaction = _context.Database.BeginTransaction();

            try
            {
                await _entitiesExemplo.AddAsync(entity);
                //var result = _entities.Add(entity);
                // System.Diagnostics.Trace.WriteLine(result.ToString());

                await _context.SaveChangesAsync();
                transaction.Commit();

                return entity;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                // Debug.WriteLine(ex.Message);

                throw new Exception(ex.Message);
            }
        }

        public async Task<int> CountAsync() => await _entitiesExemplo
            .Where(i => i.IsDeleted == false)
            .CountAsync();

        public async Task<int> CountByConditionAsync(Expression<Func<Exemplo, bool>> expression) => await _entitiesExemplo
            .Where(i => i.IsDeleted == false)
            .Where(expression)
            .CountAsync();

        public async Task<List<Exemplo>> GetAllAsync() => await _entitiesExemplo
            .Where(i => i.IsDeleted == false)
            .ToListAsync();

        public async Task<List<Exemplo>> GetAllAsync(int pageSize, int pageNumber) => await _entitiesExemplo
            .Where(i => i.IsDeleted == false)
            .OrderByDescending(i => i.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        public async Task<List<Exemplo>> FindByConditionAsync(Expression<Func<Exemplo, bool>> expression) => await _entitiesExemplo
            .Where(i => i.IsDeleted == false)
            .Where(expression)
            .ToListAsync();

        public async Task<List<Exemplo>> FindByConditionAsync(Expression<Func<Exemplo, bool>> expression, int pageSize, int pageNumber) => await _entitiesExemplo
            .Where(i => i.IsDeleted == false)
            .Where(expression)
            .OrderByDescending(i => i.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        public async Task<Exemplo> GetByIdAsync(int? id) => await _entitiesExemplo
            .Where(i => i.IsDeleted == false)
            .SingleOrDefaultAsync(s => s.Id == id);

        public async Task<Exemplo> UpdateAsync(Exemplo entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var transaction = _context.Database.BeginTransaction();

            try
            {
                if (await _entitiesExemplo.AnyAsync(i => i.Id == entity.Id && i.IsDeleted == false))
                {

                    _entitiesExemplo.Update(entity);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                else
                {
                    throw new Exception("Entity not found");
                }

                return entity;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Debug.WriteLine(ex.Message);

                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(Exemplo entity)
        {
            var transaction = _context.Database.BeginTransaction();

            try
            {
                entity.IsDeleted = true;
                _entitiesExemplo.Update(entity);
                var response = await _context.SaveChangesAsync();
                transaction.Commit();

                return (response > 0);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Debug.WriteLine(ex.Message);

                throw new Exception(ex.Message);
            }
        }

    }
}
