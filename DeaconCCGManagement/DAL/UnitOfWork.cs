using DeaconCCGManagement.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace DeaconCCGManagement.DAL
{
    /// <summary> 
    /// Allows for multiple database operations with one context and one
    /// call to SaveChanges() at the end. 
    /// "Unit of Work is a class that keeps track of all the operations in a 
    /// transaction, and then performs all the operations as a single atomic unit." 
    /// -- Mastering Entity Framework p. 259
    /// </summary>
    public class UnitOfWork : IDisposable
    {
        private CcgDbContext context;
        private IMemberRepository<CCGMember> _memberRepository;
        private IAppUserRepository<CCGAppUser> _appUserRepository;
        private IContactTypeRepository<ContactType> _contactTypeRepository;
        private ICCGRepository<CCG> _ccgRepository;
        private IRoleRepository<IdentityRole> _roleRepository;
        private IContactRecordRepository<ContactRecord> _contactRecordRepository;        
        private IPassAlongContactRepository<PassAlongContact> _passAlongContactRepository;
        private IChangeRequestRepository<ChangeRequest> _changeRequestRepository;
        private INeedsCommunionRepository<NeedsCommunion> _needsCommunionRepository;

        public UnitOfWork()
        {
            context = new CcgDbContext();
        }

        public IMemberRepository<CCGMember> MemberRepository
        {
            get
            {
                return _memberRepository ?? 
                    (_memberRepository = new MemberRepository(context));              
            }
        }

        public IAppUserRepository<CCGAppUser> AppUserRepository
        {
            get
            {               
                return _appUserRepository ??
                  (_appUserRepository = new AppUserRepository(context));
            }
        }

        public IContactTypeRepository<ContactType> ContactTypeRepository
        {
            get
            {
                return _contactTypeRepository ??
                    (_contactTypeRepository = new ContactTypeRepository(context));
            }
        }

        public ICCGRepository<CCG> CCGRepository
        {
            get
            {
                return _ccgRepository ??
                    (_ccgRepository = new CCGRepository(context));
            }
        }

        public IRoleRepository<IdentityRole> RoleRepository
        {
            get
            {
                return _roleRepository ??
                    (_roleRepository = new RoleRepository(context));
            }
        }

        public IContactRecordRepository<ContactRecord> ContactRecordRepository
        {
            get
            {
                return _contactRecordRepository ??
                       (_contactRecordRepository = new ContactRecordRepository(context));
            }
        }

        public IPassAlongContactRepository<PassAlongContact> PassAlongContactRepository
        {
            get
            {
                return _passAlongContactRepository ??
                       (_passAlongContactRepository = new PassAlongContactRepository(context));
            }
        }

        public IChangeRequestRepository<ChangeRequest> ChangeRequestRepository
        {
            get
            {
                return _changeRequestRepository ??
                       (_changeRequestRepository = new ChangeRequestRepository(context));
            }
        }

        public INeedsCommunionRepository<NeedsCommunion> NeedsCommunionRepository
        {
            get
            {
                return _needsCommunionRepository ??
                       (_needsCommunionRepository = new NeedsCommunionRepository(context));
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        #region dispose object
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)            
                if (disposing)                
                    context.Dispose();               
            
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}