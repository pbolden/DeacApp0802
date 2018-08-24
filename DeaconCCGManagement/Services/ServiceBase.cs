using DeaconCCGManagement.DAL;

namespace DeaconCCGManagement.Services
{
    public abstract class ServiceBase
    {
        protected UnitOfWork unitOfWork;

        protected ServiceBase(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }       
    }
}