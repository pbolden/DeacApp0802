using AutoMapper;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.ViewModels;


namespace DeaconCCGManagement.Infrastructure
{
    /// <summary>
    /// Initializes maps for AutoMapper.
    /// AutoMapper maps the domain (data) models to the
    /// view models and vice versa. e.g., Member ==> MemberViewModel
    /// </summary>
    public class AutoMapperBootstrapper
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                // CCGMember profiles
                cfg.AddProfile<ListMembersViewModelProfile>();
                cfg.AddProfile<CreateMemberViewModelProfile>();
                cfg.AddProfile<EditMemberViewModelProfile>();
                cfg.AddProfile<DetailsMemberViewModelProfile>();
                cfg.AddProfile<DeleteMemberViewModelProfile>();
                cfg.AddProfile<CreateMemberToMemberProfile>();
                cfg.AddProfile<EditMemberToMemberProfile>();

                // User profiles
                cfg.AddProfile<CcgAppUserViewModelProfile>();
                cfg.AddProfile<CreateUserViewModelProfile>();
                cfg.AddProfile<CreateUserToUserViewModelProfile>();
                cfg.AddProfile<DeleteUserViewModelProfile>();
                cfg.AddProfile<DetailsUserViewModelProfile>();
                cfg.AddProfile<EditUserViewModelProfile>();
                cfg.AddProfile<EditUserToUserViewModelProfile>();

                // ContactRecord profiles
                cfg.AddProfile<ListContactRecordViewModelProfile>();
                cfg.AddProfile<DeleteContactRecordViewModelProfile>();
                cfg.AddProfile<DetailsContactRecordViewModelProfile>();
                cfg.AddProfile<EditContactRecordViewModelProfile>();
                cfg.AddProfile<EditContactRecordVMToContactRecordProfile>();
                cfg.AddProfile<CreateContactRecordViewModelProfile>();
                cfg.AddProfile<CreateContactRecordVMToContactRecordProfile>();

                // PassAlongContacts profiles
                cfg.AddProfile<PassAlongContactRecordsViewModelProfile>();
                cfg.AddProfile<DeletePassAlongContactViewModelProfile>();
                cfg.AddProfile<PassAlongFollowUpViewModelProfile>();

                // CCG profiles
                cfg.AddProfile<CCGViewModelProfile>();

                // Prayer request profiles
                cfg.AddProfile<PrayerRequestViewModelProfile>();
                cfg.AddProfile<PrayerRequestToContactRecordProfile>();

                // Change request profiles
                cfg.AddProfile<ChangeRequestViewModelProfile>();
                cfg.AddProfile<ChangeRequestToEditMemberViewModelProfile>();
            });
        }
     
        #region CCGMember profile classes

        // Maps CCGMember to ListMembersViewModel
        public class ListMembersViewModelProfile : Profile
        {
            public ListMembersViewModelProfile()
            {
                CreateMap<CCGMember, ListMembersViewModel>()
                    .ForMember(dest => dest.Selected, opt => opt.Ignore());
            }
        }

        // Maps CCGMember to CreateMemberViewModel
        public class CreateMemberViewModelProfile : Profile
        {
            public CreateMemberViewModelProfile()
            {
                CreateMap<CCGMember, CreateMemberViewModel>();
            }
        }

        // Maps CCGMember to EditMemberViewModel 
        public class EditMemberViewModelProfile : Profile
        {
            public EditMemberViewModelProfile()
            {
                CreateMap<CCGMember, EditMemberViewModel>()
                    .ForMember(dest => dest.CcgMemberId, opt => opt.MapFrom(m => m.Id));
            }
        }

        // Maps CCGMember to DetailsMemberViewModel
        public class DetailsMemberViewModelProfile : Profile
        {
            public DetailsMemberViewModelProfile()
            {
                CreateMap<CCGMember, DetailsMemberViewModel>();
            }
        }

        // Maps CCGMember to DeleteMemberViewModel
        public class DeleteMemberViewModelProfile : Profile
        {
            public DeleteMemberViewModelProfile()
            {
                CreateMap<CCGMember, DeleteMemberViewModel>();
            }
        }

        // Maps CreateMemberViewModel to CCGMember
        public class CreateMemberToMemberProfile : Profile
        {
            public IgnoreMapAttribute IgnoreMapAttribute { get; set; }

            public CreateMemberToMemberProfile()
            {
                CreateMap<CreateMemberViewModel, CCGMember>()
                    .ForMember(dest => dest.FamilyNumber, opt => opt.Ignore())
                    .ForMember(dest => dest.EnvelopNumber, opt => opt.Ignore())
                    .ForMember(dest => dest.FamDistrictDeacon, opt => opt.Ignore())
                    .ForMember(dest => dest.IndividualId, opt => opt.Ignore())
                    .ForMember(dest => dest.DateLastChanged, opt => opt.Ignore())
                    .ForMember(dest => dest.InactiveDate, opt => opt.Ignore())
                    .ForMember(dest => dest.CCG, opt => opt.Ignore());
                //.ForMember(dest => dest.ContactRecords, opt => opt.Ignore());
            }
        }

        // Maps EditMemberViewModel to CCGMember
        public class EditMemberToMemberProfile : Profile
        {
            public IgnoreMapAttribute IgnoreMapAttribute { get; set; }

            public EditMemberToMemberProfile()
            {
                CreateMap<EditMemberViewModel, CCGMember>()
                    .ForMember(dest => dest.FamilyNumber, opt => opt.Ignore())
                    .ForMember(dest => dest.EnvelopNumber, opt => opt.Ignore())
                    .ForMember(dest => dest.FamDistrictDeacon, opt => opt.Ignore())
                    .ForMember(dest => dest.IndividualId, opt => opt.Ignore())
                    .ForMember(dest => dest.DateLastChanged, opt => opt.Ignore())
                    .ForMember(dest => dest.InactiveDate, opt => opt.Ignore())
                    .ForMember(dest => dest.CCG, opt => opt.Ignore());
                //.ForMember(dest => dest.ContactRecords, opt => opt.Ignore());
            }
                    
        }
        #endregion

        #region Users profile classes

        // Maps CCGAppUser to CcgAppUserViewModel
        public class CcgAppUserViewModelProfile : Profile
        {
            public CcgAppUserViewModelProfile()
            {
                CreateMap<CCGAppUser, CcgAppUserViewModel>()
                    .ForSourceMember(src => src.UserName, opt => opt.Ignore())
                    .ForMember(dest => dest.SharePointEmail, opt => opt.MapFrom(src => src.Email));
            }
        }

        // Maps CCGAppUser to CreateCcgAppUserViewModel
        public class CreateUserViewModelProfile : Profile
        {
            public CreateUserViewModelProfile()
            {
                CreateMap<CCGAppUser, CreateCcgAppUserViewModel>()
                    .ForSourceMember(src => src.UserName, opt => opt.Ignore())
                    //.ForSourceMember(src => src.Members, opt => opt.Ignore())
                    .ForMember(dest => dest.SharePointEmail, opt => opt.MapFrom(src => src.Email));
            }
        }

        // Maps CreateCcgAppUserViewModel to CCGAppUser
        public class CreateUserToUserViewModelProfile : Profile
        {
            public CreateUserToUserViewModelProfile()
            {
                CreateMap<CreateCcgAppUserViewModel, CCGAppUser>()
                    //.ForMember(dest => dest.Members, opt => opt.Ignore())
                    .ForSourceMember(src => src.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.Id, opt => opt.Ignore()) // To let Identity generate Id
                    .ForSourceMember(src => src.FullName, opt => opt.Ignore())
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.SharePointEmail))
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.SharePointEmail));
            }
        }

        // Maps CCGAppUser to DeleteCcgAppUserViewModel
        public class DeleteUserViewModelProfile : Profile
        {
            public DeleteUserViewModelProfile()
            {
                CreateMap<CCGAppUser, DeleteCcgAppUserViewModel>()
                    .ForSourceMember(src => src.UserName, opt => opt.Ignore())
                    //.ForSourceMember(src => src.Members, opt => opt.Ignore())
                    .ForMember(dest => dest.SharePointEmail, opt => opt.MapFrom(src => src.Email));
            }
        }

        // Maps CCGAppUser to DetailsCcgAppUserViewModel
        public class DetailsUserViewModelProfile : Profile
        {
            public DetailsUserViewModelProfile()
            {
                CreateMap<CCGAppUser, DetailsCcgAppUserViewModel>()
                    .ForSourceMember(src => src.UserName, opt => opt.Ignore())
                    //.ForSourceMember(src => src.Members, opt => opt.Ignore())
                    .ForMember(dest => dest.SharePointEmail, opt => opt.MapFrom(src => src.Email));
            }
        }

        // Maps CCGAppUser to EditCcgAppUserViewModel
        public class EditUserViewModelProfile : Profile
        {
            public EditUserViewModelProfile()
            {
                CreateMap<CCGAppUser, EditCcgAppUserViewModel>()
                    .ForSourceMember(src => src.UserName, opt => opt.Ignore())
                    //.ForSourceMember(src => src.Members, opt => opt.Ignore())
                    .ForMember(dest => dest.SharePointEmail, opt => opt.MapFrom(src => src.Email));
            }
        }

        // Maps EditCcgAppUserViewModel to CCGAppUser
        public class EditUserToUserViewModelProfile : Profile
        {
            public EditUserToUserViewModelProfile()
            {
                CreateMap<EditCcgAppUserViewModel, CCGAppUser>()
                    //.ForMember(dest => dest.Members, opt => opt.Ignore())
                    .ForSourceMember(src => src.FullName, opt => opt.Ignore())
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.SharePointEmail))
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.SharePointEmail));
            }
        }

        #endregion

        #region ContactRecords profile classes
        // Maps ContactRecord to ContactRecordViewModel
        public class ListContactRecordViewModelProfile : Profile
        {
            public ListContactRecordViewModelProfile()
            {
                CreateMap<ContactRecord, ContactRecordViewModel>()
                    .ForSourceMember(src => src.DeaconFullName, opt => opt.Ignore())
                    .ForSourceMember(src => src.MemberFullName, opt => opt.Ignore());
            }
        }

        // Maps ContactRecord to DeleteContactRecordViewModel 
        public class DeleteContactRecordViewModelProfile : Profile
        {
            public DeleteContactRecordViewModelProfile()
            {
                CreateMap<ContactRecord, DeleteContactRecordViewModel>();
            }
        }

        // Maps ContactRecord to DetailsContactRecordViewModel
        public class DetailsContactRecordViewModelProfile : Profile
        {
            public DetailsContactRecordViewModelProfile()
            {
                CreateMap<ContactRecord, DetailsContactRecordViewModel>();
            }
        }

        // Maps ContactRecord to EditContactRecordViewModel 
        public class EditContactRecordViewModelProfile : Profile
        {
            public EditContactRecordViewModelProfile()
            {
                CreateMap<ContactRecord, EditContactRecordViewModel>();
            }
        }

        // Maps EditContactRecordViewModel to ContactRecord  
        public class EditContactRecordVMToContactRecordProfile : Profile
        {
            public EditContactRecordVMToContactRecordProfile()
            {
                CreateMap<EditContactRecordViewModel, ContactRecord>();
            }
        }

        // Maps ContactRecord to CreateContactRecordViewModel
        public class CreateContactRecordViewModelProfile : Profile
        {
            public CreateContactRecordViewModelProfile()
            {
                CreateMap<ContactRecord, CreateContactRecordViewModel>()
                    .ForSourceMember(src => src.DeaconFullName, opt => opt.Ignore())
                    .ForSourceMember(src => src.MemberFullName, opt => opt.Ignore());
            }
        }
      
        // Maps CreateContactRecordViewModel to ContactRecord
        public class CreateContactRecordVMToContactRecordProfile : Profile
        {
            public CreateContactRecordVMToContactRecordProfile()
            {
                CreateMap<CreateContactRecordViewModel, ContactRecord>();
            }
        }
        #endregion

        #region PassAlongContacts profile classes
        // Maps ContactRecord to PassAlongContactRecordsViewModel
        public class PassAlongContactRecordsViewModelProfile : Profile
        {
            public PassAlongContactRecordsViewModelProfile()
            {
                CreateMap<ContactRecord, PassAlongContactRecordViewModel>();
            }
        }

        // Maps ContactRecord to DeletePassAlongContactViewModel 
        public class DeletePassAlongContactViewModelProfile : Profile
        {
            public DeletePassAlongContactViewModelProfile()
            {
                CreateMap<ContactRecord, DeletePassAlongContactViewModel>();
            }
        }

        // Maps ContactRecord to PassAlongFollowUpViewModel 
        public class PassAlongFollowUpViewModelProfile : Profile
        {
            public PassAlongFollowUpViewModelProfile()
            {
                CreateMap<ContactRecord, PassAlongFollowUpViewModel>();
            }
        }
        #endregion

        #region CCG profile classes
        // Maps CCG to CCGViewModel 
        public class CCGViewModelProfile : Profile
        {
            public CCGViewModelProfile()
            {
                CreateMap<CCG, CCGViewModel>();
            }
        }
        #endregion

        #region Prayer request profile classes
        // Maps ContactRecord to PrayerRequestViewModel
        public class PrayerRequestViewModelProfile : Profile
        {
            public PrayerRequestViewModelProfile()
            {
                CreateMap<ContactRecord, PrayerRequestViewModel>();
            }
        }

        // Maps PrayerRequestViewModel to ContactRecord
        public class PrayerRequestToContactRecordProfile : Profile
        {
            public PrayerRequestToContactRecordProfile()
            {
                CreateMap<PrayerRequestViewModel, ContactRecord>();
            }
        }
        #endregion

        #region Change request profile classes
        // Maps EditMemberViewModel to ChangeRequest
        public class ChangeRequestViewModelProfile : Profile
        {
            public ChangeRequestViewModelProfile()
            {
                CreateMap<EditMemberViewModel, ChangeRequest>();
            }
        }

        // Maps ChangeRequest to EditMemberViewModel
        public class ChangeRequestToEditMemberViewModelProfile : Profile
        {
            public ChangeRequestToEditMemberViewModelProfile()
            {
                CreateMap<ChangeRequest, EditMemberViewModel>();
            }
        }
        #endregion
    }
}