using DeaconCCGManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DeaconCCGManagement.DAL
{
    public interface IContactRecordRepository<TEntity>
    {
        void AddOrUpdate(TEntity entity);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        //TEntity FindById(int? id); // overridden
        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate = null);
        TEntity Find(Expression<Func<TEntity, bool>> predicate, bool firstOrDefault = false);
        bool Exists(Expression<Func<TEntity, bool>> predicate);
        int Count();
        void Save(CcgDbContext context);

        // unique to this repository
        ContactRecord FindById(int? id);
        IEnumerable<ContactRecord> GetContactRecords(Expression<Func<ContactRecord, bool>> predicate = null,
            int pageIndex = 0, int pageSize = 0);
        void Add(ContactRecord entity);
        void AddOrUpdate(ContactRecord entity);
        void AddToPassAlongRecords(ContactRecord entity);
        void UpdateLastContactDate(ContactRecord entity);
        void Update(ContactRecord entity);
        void Delete(ContactRecord entity);
        void AddPassAlongContact(ContactRecord entity);
    }
}
