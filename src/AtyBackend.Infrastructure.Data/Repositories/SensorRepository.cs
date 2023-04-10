using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Interfaces;
using AtyBackend.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace AtyBackend.Infrastructure.Data.Repositories
{
    public class SensorRepository : ISensorRepository
    {
        private ApplicationDbContext _context;
        private readonly DbSet<Sensor> _entitiesSensor;

        public SensorRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _entitiesSensor = context.Set<Sensor>();
        }


        public async Task<Sensor> CreateAsync(Sensor entity)
        {
            var transaction = _context.Database.BeginTransaction();

            try
            {
                await _entitiesSensor.AddAsync(entity);
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

        public async Task<int> CountAsync() => await _entitiesSensor
            .CountAsync();

        public async Task<int> CountByConditionAsync(Expression<Func<Sensor, bool>> expression) => await _entitiesSensor
            .Where(expression)
            .CountAsync();

        public async Task<List<Sensor>> GetAllAsync() => await _entitiesSensor
            .ToListAsync();

        public async Task<List<Sensor>> GetAllAsync(int pageSize, int pageNumber) => await _entitiesSensor
            .OrderByDescending(i => i.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        public async Task<List<Sensor>> FindByConditionAsync(Expression<Func<Sensor, bool>> expression) => await _entitiesSensor
            .Where(expression)
            .ToListAsync();

        public async Task<List<Sensor>> FindByConditionAsync(Expression<Func<Sensor, bool>> expression, int pageSize, int pageNumber) => await _entitiesSensor
            .Where(expression)
            .OrderByDescending(i => i.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        public async Task<Sensor> GetByIdAsync(int? id) => await _entitiesSensor
            .SingleOrDefaultAsync(s => s.Id == id);

        public async Task<Sensor> UpdateAsync(Sensor entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var transaction = _context.Database.BeginTransaction();

            try
            {
                if (await _entitiesSensor.AnyAsync(i => i.Id == entity.Id))
                {

                    _entitiesSensor.Update(entity);
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

        public async Task<bool> DeleteAsync(Sensor entity)
        {
            var transaction = _context.Database.BeginTransaction();

            try
            {
                entity.IsDeleted = true;
                _entitiesSensor.Update(entity);
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
