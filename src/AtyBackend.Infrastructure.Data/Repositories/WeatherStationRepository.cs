﻿using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Interfaces;
using AtyBackend.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

/* TODO CreateAsync
- fazer o insert da forma correta, com os for para inserir as entities filhas
- fazer o insert de .. WeatherStationSensor
- fazer o insert de .. WeatherStationSensorUser
- fazer o insert de .. Partners

- fazer os includes tipo na import
- não sei se precisa dar include em usuários, em todos ou só em algum
- ter usuário na estação com flag tipo 'dono' e só mostrar ele na estação ou mostrar todos?
- quando mostrar, mostrar o nome do usuário ou o email? Quais informações? Faço uma nova DTO/view model
  para mostrar só algumas informações do usuário e no AutoMapping eu mapeio isso noentity -> DTO

GET basico com os includes para testes

 */

namespace AtyBackend.Infrastructure.Data.Repositories
{
    public class WeatherStationRepository : IWeatherStationRepository
    {
        private ApplicationDbContext _context;
        private readonly DbSet<WeatherStation> _entitiesWeatherStation;
        private readonly DbSet<WeatherStationSensor> _entitiesWeatherStationSensor;
        private readonly DbSet<WeatherStationUser> _entitiesWeatherStationUser;

        public WeatherStationRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _entitiesWeatherStation = context.Set<WeatherStation>();
            _entitiesWeatherStationSensor = context.Set<WeatherStationSensor>();
            _entitiesWeatherStationUser = context.Set<WeatherStationUser>();
        }


        public async Task<WeatherStation> CreateAsync(WeatherStation entity)
        {
            var transaction = _context.Database.BeginTransaction();

            try
            {
                await _entitiesWeatherStation.AddAsync(entity);
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

        public async Task<int> CountAsync() => await _entitiesWeatherStation
            .CountAsync();

        public async Task<int> CountByConditionAsync(Expression<Func<WeatherStation, bool>> expression) => await _entitiesWeatherStation
            .Where(expression)
            .CountAsync();

        public async Task<List<WeatherStation>> GetAllAsync() => await _entitiesWeatherStation
            .ToListAsync();

        public async Task<List<WeatherStation>> GetAllAsync(int pageSize, int pageNumber) => await _entitiesWeatherStation
            .OrderByDescending(i => i.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        public async Task<List<WeatherStation>> FindByConditionAsync(Expression<Func<WeatherStation, bool>> expression) => await _entitiesWeatherStation
            .Where(expression)
            .ToListAsync();

        public async Task<List<WeatherStation>> FindByConditionAsync(Expression<Func<WeatherStation, bool>> expression, int pageSize, int pageNumber) => await _entitiesWeatherStation
            .Where(expression)
            .OrderByDescending(i => i.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        public async Task<WeatherStation> GetByIdAsync(int? id) => await _entitiesWeatherStation
            .SingleOrDefaultAsync(s => s.Id == id);

        public async Task<WeatherStation> UpdateAsync(WeatherStation entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var transaction = _context.Database.BeginTransaction();

            try
            {
                if (await _entitiesWeatherStation.AnyAsync(i => i.Id == entity.Id))
                {

                    _entitiesWeatherStation.Update(entity);
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

        public async Task<bool> DeleteAsync(WeatherStation entity)
        {
            var transaction = _context.Database.BeginTransaction();

            try
            {
                entity.IsDeleted = true;
                _entitiesWeatherStation.Update(entity);
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