using PagedList;

namespace DeaconCCGManagement.ViewModels
{
    /// <summary>
    /// Pass along to pastor/leadership contact record view models
    /// </summary>
    public class PassAlongContactRecordViewModel
    {
        public IPagedList<ContactRecordViewModel> ContactRecords { get; set; }
        public ActionMethodParams Params { get; set; }
    }

    public class DeletePassAlongContactViewModel : ContactRecordViewModel
    {

    }

    public class PassAlongFollowUpViewModel : ContactRecordViewModel
    {

    }
}