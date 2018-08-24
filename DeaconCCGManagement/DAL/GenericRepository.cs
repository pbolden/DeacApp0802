using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;

namespace DeaconCCGManagement.DAL
{
    public abstract class GenericRepository<TEntity> where TEntity : class 
    {
        internal CcgDbContext context;
        internal DbSet<TEntity> dbSet;


        protected GenericRepository(CcgDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();

        }

        /// <summary>
        /// Only use when seeding data using migration.
        /// </summary>
        /// <param name="entity"></param> 
        public virtual void AddOrUpdate(TEntity entity)
        {           
            var dbSet = context.Set<TEntity>();
            dbSet.AddOrUpdate(entity);
            Save(context);        
        }

        public virtual void Add(TEntity entity)
        {          
            var dbSet = context.Set<TEntity>();
            dbSet.Add(entity);
            Save(context);            
        }

        public virtual void Update(TEntity entity)
        {            
            context.Entry(entity).State = EntityState.Modified;
            Save(context);
            
        } 

    public virtual void Delete(TEntity entity)
        {           
            var dbSet = context.Set<TEntity>();
            context.Entry(entity).State = EntityState.Deleted;
            dbSet?.Remove(entity);
            Save(context);            
        }

        public virtual TEntity FindById(int? id)
        {          
            var dbSet = context.Set<TEntity>();
            return dbSet.Find(id);            
        }

        public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate = null)
        {           
            var dbSet = context.Set<TEntity>();
            return predicate != null ? dbSet.Where(predicate).AsEnumerable().ToList() : dbSet.AsEnumerable().ToList();
            
        }

        public virtual TEntity Find(Expression<Func<TEntity, bool>> predicate, bool firstOrDefault=false)
        {            
            var dbSet = context.Set<TEntity>();
            return firstOrDefault ? dbSet.FirstOrDefault(predicate)
            : dbSet.SingleOrDefault(predicate);            
        }

        public virtual bool Exists(Expression<Func<TEntity, bool>> predicate)
        {           
            var dbSet = context.Set<TEntity>();
            return predicate != null && dbSet.Any(predicate);            
        }

        public virtual int Count()
        {           
            var dbSet = context.Set<TEntity>();
            return dbSet.Count();            
        }

        public virtual void Save(CcgDbContext context)
        {
            context.SaveChanges();
        }
    }
}