using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DaymsWPFBoiler.Data.Respositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DaymsWPFBoiler.Data.Respositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;

        public Repository(DbContext context)
        {
            Context = context;
        }

        public void Update(TEntity entity)
        {
            try
            {
                Context.Set<TEntity>().Update(entity);
            }
            catch (InvalidOperationException tracking)
            {
                Console.WriteLine(tracking);
            }
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().UpdateRange(entities);
        }

        public void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().AddRange(entities);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().FirstOrDefault(predicate);
        }

        public TEntity Get(Guid id)
        {
            return Context.Set<TEntity>().Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Context.Set<TEntity>().ToList();
        }

        public void Remove(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().RemoveRange(entities);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().SingleOrDefault(predicate);
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Any(predicate);
        }
    }
}
