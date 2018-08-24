using System.Web.Mvc;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.Helpers;


namespace DeaconCCGManagement.Controllers
{
    public abstract class ControllerBase : Controller
    {
        protected UnitOfWork unitOfWork;

        protected ControllerBase()
        {
            unitOfWork = new UnitOfWork();
        }

        /// <summary>
        /// Maps data models to view models. Configured in AutoMapperBootstraper.
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="domainModel"></param>
        /// <param name="viewResult"></param>
        /// <returns></returns>
        protected AutoMapViewResult AutoMapView<TDestination>(object domainModel, ViewResultBase viewResult)
        {
            return new AutoMapViewResult(domainModel.GetType(), typeof(TDestination), domainModel, viewResult);
        }

        protected override void Dispose(bool disposing)
        {            
            unitOfWork.Dispose(); 
            base.Dispose(disposing);
        }
    }
}