using AutoMapper;
using Esso.Models;
using Esso.ViewModels;
using Esso.Web.Models;
using Esso.Web.ViewModels;
using System;

namespace Esso.Web.Mappings
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<RegisterViewModel, ApplicationUser>()
            .ForMember(g => g.Id, map => map.MapFrom(vm => vm.ID))
            .ForMember(g => g.UserName, map => map.MapFrom(vm => vm.UserName))
            .ForMember(g => g.Email, map => map.MapFrom(vm => vm.Email))
			//.ForMember(g => g.SHOW_MONEY, map => map.MapFrom(vm => vm.SHOW_MONEY))
            .ForMember(g => g.PasswordHash, map => map.MapFrom(vm => vm.Password));


            CreateMap<TagDTO, TBL_TAG>()
          .ForMember(g => g.ID, map => map.MapFrom(vm => vm.ID))
          .ForMember(g => g.NAME, map => map.MapFrom(vm => vm.NAME))
          .ForMember(g => g.IS_DIGITAL, map => map.MapFrom(vm => vm.IS_DIGITAL))
          .ForMember(g => g.IS_STRING, map => map.MapFrom(vm => vm.IS_STRING))
          .ForMember(g => g.IS_INV_TAG, map => map.MapFrom(vm => vm.IS_INV_TAG));


            CreateMap<StationGridModel, TBL_STATION>()
           .ForMember(g => g.COMPANY_ID, map => map.MapFrom(vm => vm.COMPANY_ID))
           .ForMember(g => g.CREATED_DATE, map => map.MapFrom(vm => vm.CREATED_DATE))
           .ForMember(g => g.GROUP_ID, map => map.MapFrom(vm => vm.GROUP_ID))
           .ForMember(g => g.INSTALL_DATE, map => map.MapFrom(vm => vm.INSTALL_DATE))
           .ForMember(g => g.IS_ACTIVE, map => map.MapFrom(vm => vm.IS_ACTIVE))
           .ForMember(g => g.IS_DELETED, map => map.MapFrom(vm => vm.IS_DELETED))
           .ForMember(g => g.IS_LOCKED, map => map.MapFrom(vm => vm.IS_LOCKED))
           .ForMember(g => g.IP_ADDRESS, map => map.MapFrom(vm => vm.IP_ADDRESS))
           .ForMember(g => g.PORT, map => map.MapFrom(vm => vm.PORT))
           .ForMember(g => g.METEROROLOGY_PLANT, map => map.MapFrom(vm => vm.METEROROLOGY_PLANT))           
           .ForMember(g => g.IS_CENTRAL_INV, map => map.MapFrom(vm => vm.IS_CENTRAL_INV))
           .ForMember(g => g.NAME, map => map.MapFrom(vm => vm.NAME))
           .ForMember(g => g.START_DATE, map => map.MapFrom(vm => vm.START_DATE))
           .ForMember(g => g.ORIENTATION, map => map.MapFrom(vm => vm.ORIENTATION))
           .ForMember(g => g.PITCH, map => map.MapFrom(vm => vm.PITCH))
           .ForMember(g => g.PANEL_BRAND, map => map.MapFrom(vm => vm.PANEL_BRAND))
           .ForMember(g => g.PANEL_TYPE, map => map.MapFrom(vm => vm.PANEL_TYPE))

           .ForMember(g => g.WEATHER_LOCATION, map => map.MapFrom(vm => vm.WEATHER_LOCATION))
           .ForMember(g => g.AC_INSTALLED_POWER, map => map.MapFrom(vm => vm.AC_INSTALLED_POWER))
           .ForMember(g => g.DC_INSTALLED_POWER, map => map.MapFrom(vm => vm.DC_INSTALLED_POWER))
           .ForMember(g => g.INVERTER_MODEL, map => map.MapFrom(vm => vm.INVERTER_MODEL))
           .ForMember(g => g.EXCHANGE_RATE, map => map.MapFrom(vm => vm.EXCHANGE_RATE))
            .ForMember(g => g.TAX, map => map.MapFrom(vm => vm.TAX))
           .ForMember(g => g.COORDINATE_INFORMATION, map => map.MapFrom(vm => vm.COORDINATE_INFORMATION))

           .ForMember(g => g.SIZE, map => map.MapFrom(vm => vm.SIZE))
           .ForMember(g => g.ADDRESS, map => map.MapFrom(vm => vm.ADDRESS))
           .ForMember(g => g.DESCRIPTION, map => map.MapFrom(vm => vm.DESCRIPTION))
           .ForMember(g => g.UPDATED_DATE, map => map.MapFrom(vm => vm.UPDATED_DATE))
           .ForMember(g => g.UPDATE_USER, map => map.MapFrom(vm => vm.UPDATE_USER))
           .ForMember(g => g.ALARM_TEMP_ID, map => map.MapFrom(vm => vm.ALARM_TEMP_ID))
           .ForMember(g => g.ID, map => map.MapFrom(vm => vm.ID));

            CreateMap<StationGroupViewModel, TBL_STATION_GROUP>()
           .ForMember(g => g.COMPANY_ID, map => map.MapFrom(vm => vm.COMPANY_ID))
           .ForMember(g => g.CREATED_DATE, map => map.MapFrom(vm => vm.CREATED_DATE))
           .ForMember(g => g.IS_DELETED, map => map.MapFrom(vm => vm.IS_DELETED))
           .ForMember(g => g.NAME, map => map.MapFrom(vm => vm.NAME))
           .ForMember(g => g.UPDATED_DATE, map => map.MapFrom(vm => vm.UPDATED_DATE))
           .ForMember(g => g.UPDATE_USER, map => map.MapFrom(vm => vm.UPDATE_USER))
           .ForMember(g => g.ID, map => map.MapFrom(vm => vm.ID));

            CreateMap<TBL_OZET, TBL_OZET_DTO>()
         .ForMember(g => g._id, map => map.MapFrom(vm => vm.Id))
         .ForMember(g => g._tarih, map => map.MapFrom(vm => vm.tarih))
         .ForMember(g => g._gunlukUretim, map => map.MapFrom(vm => vm.gunlukUretim <= 0 || vm.gunlukUretim == null ? 0 : (float)Math.Round((double)vm.gunlukUretim / 1000, 1)))
         .ForMember(g => g._dcGuc, map => map.MapFrom(vm =>vm.Dc_Guc == null ? 0 : vm.Dc_Guc))
         .ForMember(g => g._sicaklik, map => map.MapFrom(vm => vm.sicaklik == null ? 0 : (float)Math.Round((double)vm.sicaklik)))
         .ForMember(g => g._isinim, map => map.MapFrom(vm => vm.isinim==null ? 0 : (float)Math.Round((double)vm.isinim,1)))
         .ForMember(g => g._enerji, map => map.MapFrom(vm => vm.Enerji <= 0f || vm.Enerji == null ? 0f : (float)Math.Round((double)vm.Enerji / 1000000, 2)))
         .ForMember(g => g._acGuc, map => map.MapFrom(vm => vm.gunlukUretim <= 0 || vm.gunlukUretim == null ? 0 : (float)Math.Round((double)vm.gunlukUretim / 1000, 1)))
         .ForMember(g => g._hucreSicakligi, map => map.MapFrom(vm => vm.hucreSicakligi==null ? 0 : (float)Math.Round((double)vm.hucreSicakligi, 1)))        
         .ForMember(g => g._ruzgarHizi, map => map.MapFrom(vm => vm.ruzgarHizi));


            CreateMap<TBL_OZET, EndProductionModel>()
           .ForMember(g => g._tarih, map => map.MapFrom(vm => vm.tarih))
           .ForMember(g => g._pac, map => map.MapFrom(vm => vm.gunlukUretim <= 0 || vm.gunlukUretim == null ? 0 : (float)Math.Round((double)vm.gunlukUretim / 1000, 2)))
           .ForMember(g => g._dailyProduction, map => map.MapFrom(vm => vm.Enerji == null ? 0 : (float)Math.Round((double)vm.Enerji / 1000000, 2)))
           .ForMember(g => g._monthlyProduction, map => map.MapFrom(vm => vm.aylikEnerji == null ? 0 : (float)Math.Round((double)vm.aylikEnerji / 1000000, 2)))
           .ForMember(g => g._annualProduction, map => map.MapFrom(vm => vm.yillikEnerji == null ? 0 : (float)Math.Round((double)vm.yillikEnerji / 1000000, 2)))
           .ForMember(g => g._totalProduction, map => map.MapFrom(vm => vm.toplamEnerji == null ? 0 : (float)Math.Round((double)vm.toplamEnerji / 1000000, 2)))
           .ForMember(g => g._isinim, map => map.MapFrom(vm => vm.isinim == null ? 0 : (float)Math.Round((double)vm.isinim, 2)))
           .ForMember(g => g._ortamSicakligi, map => map.MapFrom(vm => vm.sicaklik == null ? 0 : (float)Math.Round((double)vm.sicaklik, 2)))
           .ForMember(g => g._hucreSicakligi, map => map.MapFrom(vm => vm.hucreSicakligi == null ? 0 : (float)Math.Round((double)vm.hucreSicakligi, 2)))
           .ForMember(g => g._ruzgar, map => map.MapFrom(vm => vm.ruzgarHizi == null ? 0 : (float)Math.Round((double)vm.ruzgarHizi, 2)))
           .ForMember(g => g._pdc, map => map.MapFrom(vm => vm.Dc_Guc == null ? 0 : (float)Math.Round((double)vm.Dc_Guc / 1000, 2)));


            CreateMap<TBL_PR_OZET, TBL_PR_OZET_DTO>()
             .ForMember(g => g._id, map => map.MapFrom(vm => vm.id))
             .ForMember(g => g._tarih, map => map.MapFrom(vm => vm.date))
             .ForMember(g => g._enerji, map => map.MapFrom(vm => vm.enerji <= 0 || vm.enerji == null ? 0 : (float)Math.Round((double)vm.enerji, 2)))
             .ForMember(g => g._isinim_ortalama, map => map.MapFrom(vm => vm.isinim_ortalama == null ? 0 : (float)Math.Round((double)vm.isinim_ortalama)))
             .ForMember(g => g._pr, map => map.MapFrom(vm => vm.pr <= 0f || vm.pr == null ? 0f : (float)Math.Round((double)vm.pr, 2)));

            CreateMap<TagTempDetGridModel, TBL_TAG_TEMP_DET>()
            .ForMember(g => g.ADDRESS, map => map.MapFrom(vm => vm.ADDRESS))
            .ForMember(g => g.ID, map => map.MapFrom(vm => vm.ID))
            .ForMember(g => g.INV_NO, map => map.MapFrom(vm => vm.INV_NO))
            .ForMember(g => g.TAG_ID, map => map.MapFrom(vm => vm.TAG_ID))
            .ForMember(g => g.TEMPLATE_ID, map => map.MapFrom(vm => vm.TEMPLATE_ID));

            CreateMap<TBL_OZET, ProductionModel>()
           .ForMember(g => g._tarih, map => map.MapFrom(vm => vm.tarih))
           .ForMember(g => g._stationId, map => map.MapFrom(vm => vm.STATION_ID))
           .ForMember(g => g._pac, map => map.MapFrom(vm => vm.gunlukUretim <= 0 || vm.gunlukUretim == null ? 0 : (float)Math.Round((double)vm.gunlukUretim / 1000, 2)))
           .ForMember(g => g._pdc, map => map.MapFrom(vm => vm.Dc_Guc == null ? 0 : (float)Math.Round((double)vm.Dc_Guc / 1000, 2)))
           .ForMember(g => g._isinim, map => map.MapFrom(vm => vm.isinim == null ? 0 : (float)Math.Round((double)vm.isinim, 2)))
           .ForMember(g => g._ortamSicakligi, map => map.MapFrom(vm => vm.sicaklik == null ? 0 : (float)Math.Round((double)vm.sicaklik, 2)))
           .ForMember(g => g._hucreSicakligi, map => map.MapFrom(vm => vm.hucreSicakligi == null ? 0 : (float)Math.Round((double)vm.hucreSicakligi, 2)))
           .ForMember(g => g._ruzgar, map => map.MapFrom(vm => vm.ruzgarHizi == null ? 0 : (float)Math.Round((double)vm.ruzgarHizi, 2)))
           .ForMember(g => g.H2_P, map => map.MapFrom(vm => vm.H2_P == null ? 0 : (float)vm.H2_P))
           .ForMember(g => g._dailyProduction, map => map.MapFrom(vm => vm.Enerji == null ? 0 : (float)Math.Round((double)vm.Enerji / 1000000, 2)));
 
        }
        public override string ProfileName
        {
            get { return "ViewModelToDomainMappings"; }
        }
        
    }
}