using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Xml;
using DeaconCCGManagement.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DeaconCCGManagement.DAL
{
    public class CcgDbContext : IdentityDbContext<CCGAppUser>
    {
        public CcgDbContext()
            : base("name=CcgDbContext")
        {       

        }

        public virtual DbSet<CCGMember> Members { get; set; }

        public virtual DbSet<CCG> CCGs { get; set; }

        public virtual DbSet<ContactRecord> ContactRecords { get; set; }

        public virtual DbSet<ContactType> ContactTypes { get; set; }

        //public virtual DbSet<MemberPhoto> MemberPhotos { get; set; }

        public virtual DbSet<PassAlongContact> PassAlongContacts { get; set; }

        public virtual DbSet<ChangeRequest> ChangeRequests { get; set; }
      
        public virtual DbSet<NeedsCommunion> NeedsCommunion { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            if (modelBuilder == null)
            {
                throw new ArgumentNullException("ModelBuilder is NULL");
            }

            base.OnModelCreating(modelBuilder);

            #region Change the names of the tables
            modelBuilder.Entity<IdentityUserRole>().ToTable("CCGAppUserRoles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("CCGLogins");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("CCGClaims");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<CCGAppUser>().ToTable("CCGAppUsers");
            #endregion

            #region Relationship configurations
            // These relationships are not required and will not cascade on delete  
            // (e.g., a member will not be deleted if the app user is deleted).

            //
            // No longer FK relationship between deacon and member
            // 
            // One-to-many relationship between CCGAppUser and CCGMember.
            //modelBuilder.Entity<CCGAppUser>()
            //    .HasMany(user => user.Members)
            //    .WithOptional(member => member.AppUser)
            //    .HasForeignKey(member => member.AppUserId)
            //    .WillCascadeOnDelete(false);

            // One-to-many relationship between CCG and CCGMember.
            modelBuilder.Entity<CCG>()
                .HasMany(ccg => ccg.CCGMembers)
                .WithOptional(member => member.CCG)
                .HasForeignKey(member => member.CcgId)
                .WillCascadeOnDelete(false);

            // One-to-many relationship between CCG and CCGAppUser.
            modelBuilder.Entity<CCG>()
                .HasMany(ccg => ccg.CCGMembers)
                .WithOptional(user => user.CCG)
                .HasForeignKey(user => user.CcgId)
                .WillCascadeOnDelete(false);

            #endregion
        }

        public static CcgDbContext Create()
        {
            return new CcgDbContext();
        }
    }
}