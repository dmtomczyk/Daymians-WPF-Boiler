using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DaymsWPFBoiler.Data.Respositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Get(Guid id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        bool Any(Expression<Func<TEntity, bool>> predicate);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity addition);
        void AddRange(IEnumerable<TEntity> additions);

        void Update(TEntity addition);
        void UpdateRange(IEnumerable<TEntity> additions);

        void Remove(TEntity removal);
        void RemoveRange(IEnumerable<TEntity> removals);
    }
}
