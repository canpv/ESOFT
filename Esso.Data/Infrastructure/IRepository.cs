using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Data.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        // Marks an entity as new
        void Add(T entity);
        // Marks an entity as modified
        void Update(T entity);
        // Bulk Update
        void UpdateBulk(Expression<Func<T, bool>> where, Expression<Func<T, T>> updateValues);
        // Marks an entity to be removed
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        // Get an entity by int id
        T GetById(int id);
        // Get an entity using delegate
        T Get(Expression<Func<T, bool>> where);
        // Gets all entities of type T
        List<T> GetAll();
        // Gets all entities of type T as query
        IQueryable<T> GetAllQuery(Expression<Func<T, bool>> where);
        // Gets entities using delegate
        List<T> GetMany(Expression<Func<T, bool>> where);
    }
}
