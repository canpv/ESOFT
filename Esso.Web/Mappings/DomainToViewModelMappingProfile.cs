using AutoMapper;
using Esso.Models;
using Esso.ViewModels;
using Esso.Web.Models;
using Esso.Web.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Esso.Web.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            //CreateMap<Category, CategoryViewModel>();
            //CreateMap<Gadget, GadgetViewModel>();
            CreateMap<ApplicationUser, RegisterViewModel>();
            CreateMap<TBL_STATION, StationGridModel>();
            CreateMap<TBL_STATION_GROUP, StationGroupViewModel>();
            CreateMap<TBL_TAG_TEMP_DET, TagTempDetGridModel>();
            CreateMap<TBL_TAG, TagDTO>();

            // Use CreateMap... Etc.. here (Profile methods are the same as configuration methods)
            CreateMap<TBL_OZET, TBL_OZET_DTO>();
            CreateMap<TBL_OZET, EndProductionModel>();
            CreateMap<TBL_PR_OZET, TBL_PR_OZET_DTO>();
        }

        public override string ProfileName
        {
            get { return "DomainToViewModelMappings"; }
        }

    }
}