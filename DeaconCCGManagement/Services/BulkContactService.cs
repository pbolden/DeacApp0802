using System.Collections.Generic;
using System.Linq;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.ViewModels;

namespace DeaconCCGManagement.Services
{
    public class BulkContactService : ServiceBase
    {
        public BulkContactService(UnitOfWork uow) : base(uow)
        {
        }
        /// <summary>
        /// Adds selected members to a BulkContactMember list.
        /// This only holds the members ids and names, which is all that
        /// gets pass to the view and sent back to the server.
        /// </summary>
        /// <param name="selectedMembersVM"></param>
        /// <param name="bulkContactMembers"></param>
        public void AddToBulkContactMembers(IList<ListMembersViewModel> selectedMembersVM, 
            List<BulkContactMember> bulkContactMembers)
        {
            bulkContactMembers.AddRange(selectedMembersVM.Select(member => new BulkContactMember
            {
                MemberId = member.Id, MemberFullName = member.FullName
            }));
        }
    }
}