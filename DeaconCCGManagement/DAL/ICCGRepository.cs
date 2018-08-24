using DeaconCCGManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DeaconCCGManagement.DAL
{
    public interface ICCGRepository<TEntity>
    {
        void AddOrUpdate(TEntity entity);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        //TEntity FindById(int? id); // overridden
        //IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate = null);
        TEntity Find(Expression<Func<TEntity, bool>> predicate, bool firstOrDefault = false);
        bool Exists(Expression<Func<TEntity, bool>> predicate);
        int Count();
        void Save(CcgDbContext context);

        // Unique to this repository
        IEnumerable<CCG> FindAll(Expression<Func<CCG, bool>> predicate = null);
        CCG Find(Expression<Func<CCG, bool>> predicate, bool firstOrDefault = false);
        CCG FindById(int? id);
    }
}
