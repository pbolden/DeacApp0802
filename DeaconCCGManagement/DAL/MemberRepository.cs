using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DeaconCCGManagement.Models;

namespace DeaconCCGManagement.DAL
{
    /// <summary>
    /// Data store logic and CRUD operations for members.
    /// </summary>
    public class MemberRepository : GenericRepository<CCGMember>, IMemberRepository<CCGMember>
    {
        public MemberRepository(CcgDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Returns members filtered by expression or
        /// returns all members if no expression is passed.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<CCGMember> FindMembers(Expression<Func<CCGMember, bool>> predicate = null)
        {

            var dbSet = context.Set<CCGMember>();

            if (predicate != null)
                return dbSet.Include("CCG").Where(predicate).ToList();
            return dbSet.Include("CCG").AsEnumerable().ToList();


        }
        /// <summary>
        /// Returns member by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CCGMember FindMemberById(int? id)
        {
            if (id == null)
                return null;

            var dbSet = context.Set<CCGMember>();

            return dbSet.Include("CCG").SingleOrDefault(m => m.Id == id);

        }

        /// <summary>
        /// Returns member by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public CCGMember FindMemberByIdLazyLoading(int? id, CcgDbContext dbContext)
        {
            if (id == null)
                return null;

            var dbSet = context.Set<CCGMember>();
            return dbSet.Include("CCG").SingleOrDefault(m => m.Id == id);
        }

        public IEnumerable<CCGMember> GetMemberContactRecords(int? id)
        {
            if (id == null)
                return null;


            var dbSet = context.Set<CCGMember>();

            return dbSet.Include("CCG")
                .Include("ContactRecords").AsEnumerable();

        }

        public override void Delete(CCGMember entity)
        {
            // Remove all contacts and 'pass along' records related to member
            // before deleting the member.


            var contactRecords = context.ContactRecords.Where(c => c.CCGMemberId == entity.Id);
            context.ContactRecords.RemoveRange(contactRecords);
            foreach (var contactRecord in contactRecords)
            {
                var passAlongContact = context.PassAlongContacts.SingleOrDefault(p => p.Id == contactRecord.Id);
                if (passAlongContact != null)
                {
                    context.PassAlongContacts.Remove(passAlongContact);
                }

            }


            base.Delete(entity);
        }
    }
}