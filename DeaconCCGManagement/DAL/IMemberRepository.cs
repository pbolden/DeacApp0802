using DeaconCCGManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DeaconCCGManagement.DAL
{
    public interface IMemberRepository<TEntity>
    {
        //void MemberRepository(CcgDbContext context);

        void AddOrUpdate(TEntity entity);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        TEntity FindById(int? id);
        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate = null);
        TEntity Find(Expression<Func<TEntity, bool>> predicate, bool firstOrDefault = false);
        bool Exists(Expression<Func<TEntity, bool>> predicate);
        int Count();
        void Save(CcgDbContext context);

        // unique to this repository
        IEnumerable<CCGMember> FindMembers(Expression<Func<CCGMember, bool>> predicate = null);
        CCGMember FindMemberById(int? id);
        CCGMember FindMemberByIdLazyLoading(int? id, CcgDbContext dbContext);
        IEnumerable<CCGMember> GetMemberContactRecords(int? id);
        void Delete(CCGMember entity);
    }
}
