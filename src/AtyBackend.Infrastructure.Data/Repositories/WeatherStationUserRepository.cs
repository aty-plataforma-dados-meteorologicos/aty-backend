using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Interfaces;
using AtyBackend.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace AtyBackend.Infrastructure.Data.Repositories
{
    public class WeatherStationUserRepository : IWeatherStationUserRepository
    {
        private ApplicationDbContext _context;
        private readonly DbSet<WeatherStationUser> _entitiesWeatherStationUser;

        public WeatherStationUserRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _entitiesWeatherStationUser = context.Set<WeatherStationUser>();
        }


        public async Task<WeatherStationUser> CreateAsync(WeatherStationUser entity)
        {
            var transaction = _context.Database.BeginTransaction();

            try
            {
                await _entitiesWeatherStationUser.AddAsync(entity);
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

        public async Task<int> CountAsync() => await _entitiesWeatherStationUser
            .CountAsync();

        public async Task<int> CountByConditionAsync(Expression<Func<WeatherStationUser, bool>> expression) => await _entitiesWeatherStationUser
            .Where(expression)
            .CountAsync();

        public async Task<List<WeatherStationUser>> GetAllAsync() => await _entitiesWeatherStationUser
            .ToListAsync();

        public async Task<List<WeatherStationUser>> GetAllAsync(int pageSize, int pageNumber) => await _entitiesWeatherStationUser
            //.OrderByDescending(i => i.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        public async Task<List<WeatherStationUser>> FindByConditionAsync(Expression<Func<WeatherStationUser, bool>> expression) => await _entitiesWeatherStationUser
            .Where(expression)
            .ToListAsync();

        public async Task<List<WeatherStationUser>> FindByConditionAsync(Expression<Func<WeatherStationUser, bool>> expression, int pageSize, int pageNumber) => await _entitiesWeatherStationUser
            .Where(expression)
            //.OrderByDescending(i => i.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        public async Task<WeatherStationUser> GetByIdAsync(int? id) => throw new NotImplementedException();
        //await _entitiesWeatherStationUser.SingleOrDefaultAsync(s => s.Id == id);

        public async Task<WeatherStationUser> UpdateAsync(WeatherStationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var transaction = _context.Database.BeginTransaction();

            try
            {
                var weatherStationUser = _entitiesWeatherStationUser.FirstOrDefault(wsu =>
                    wsu.WeatherStationId == entity.WeatherStationId &&
                    wsu.ApplicationUserId == entity.ApplicationUserId);


                if (weatherStationUser is not null)
                {
                    // buscar todos os user dessa estação
                    var totalWeatherStationUsers = await _entitiesWeatherStationUser
                        .Where(wsu => wsu.WeatherStationId == entity.WeatherStationId && wsu.IsMaintainer)
                        .CountAsync();

                    if (totalWeatherStationUsers > 1)
                    {
                        weatherStationUser.IsMaintainer = entity.IsMaintainer;
                        weatherStationUser.IsDataAuthorized = entity.IsDataAuthorized;
                        weatherStationUser.IsFavorite = entity.IsFavorite;
                    } else
                    {
                        weatherStationUser.IsDataAuthorized = entity.IsDataAuthorized;
                        weatherStationUser.IsFavorite = entity.IsFavorite;
                    }

                    _entitiesWeatherStationUser.Update(weatherStationUser);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                else
                {
                    throw new Exception("Entity not found");
                }

                return weatherStationUser;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Debug.WriteLine(ex.Message);

                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(WeatherStationUser entity)
        {
            var transaction = _context.Database.BeginTransaction();

            try
            {
                entity.IsDeleted = true;
                _entitiesWeatherStationUser.Update(entity);
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
