using Esso.Data.Infrastructure;
using Esso.Data.Repositories;
using Esso.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Service
{
    // operations you want to expose
    public interface IStationGroupService
    {
        List<TBL_STATION_GROUP> GetStationGroups();
        List<TBL_STATION_GROUP> GetStationGroups(Expression<Func<TBL_STATION_GROUP, bool>> where);

        void CreateStationGroup(TBL_STATION_GROUP stationGroup);
        void SaveStationGroup();
        bool IsStationGroupExist(TBL_STATION_GROUP stationGroup);
        void UpdateStationGroup(TBL_STATION_GROUP stationGroup);
        void DeleteStationGroup(int stationGroupId,string userId);
    }

    public  class StationGroupService : IStationGroupService
    {
        private static   IStationGroupRepository stationGroupRepository;
        private readonly IUnitOfWork unitOfWork;

        public StationGroupService(IStationGroupRepository _stationGroupRepository, IUnitOfWork unitOfWork)
        {
           stationGroupRepository = _stationGroupRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IStationGroupService Members

    

        public List<TBL_STATION_GROUP> GetStationGroups()
        {
            return stationGroupRepository.GetAll();
        }

        public List<TBL_STATION_GROUP> GetStationGroups(Expression<Func<TBL_STATION_GROUP, bool>> where)
        {
            return stationGroupRepository.GetMany(where);
        }

        public static List<TBL_STATION_GROUP> GetStationGroupsByCompanyId(int companyId)
        {
            return stationGroupRepository.GetMany(x => x.IS_DELETED == false && x.COMPANY_ID == companyId).ToList();
        }

        public bool IsStationGroupExist(TBL_STATION_GROUP stationGroup)
        {            
            return stationGroupRepository.Get(x => x.IS_DELETED == false && x.ID != stationGroup.ID && stationGroup.COMPANY_ID == x.COMPANY_ID && x.NAME.ToUpper() == stationGroup.NAME.ToUpper()) == null ? false : true;
        }

        public void UpdateStationGroup(TBL_STATION_GROUP stationGroup)
        {
            stationGroupRepository.Update(stationGroup);
        }

        public void DeleteStationGroup(int stationGroupId, string userId)
        {
            TBL_STATION_GROUP sg = stationGroupRepository.GetById(stationGroupId);
            sg.UPDATE_USER = userId;
            sg.IS_DELETED = true;
            stationGroupRepository.Update(sg);
        }


        //public Category GetCategory(int id)
        //{
        //    var category = categorysRepository.GetById(id);
        //    return category;
        //}

        //public Category GetCategory(string name)
        //{
        //    var category = categorysRepository.GetCategoryByName(name);
        //    return category;
        //}

        public void CreateStationGroup(TBL_STATION_GROUP stationGroup)
        {
            stationGroupRepository.Add(stationGroup);
        }

        public void SaveStationGroup()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
