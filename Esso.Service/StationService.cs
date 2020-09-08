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
    public interface IStationService
    {
        List<TBL_STATION> GetStations();
        List<TBL_STATION> GetStations(Expression<Func<TBL_STATION, bool>> where);

        void CreateStation(TBL_STATION station);
        void SaveStation();
        List<TBL_STATION> GetStationByCompanyId(int companyId);
        bool IsStationExist(TBL_STATION station);
        void UpdateStation(TBL_STATION station);
        //void UpdateStation(Expression<Func<TBL_STATION, bool>> where, Expression<Func<TBL_STATION, TBL_STATION>> updateValues);
        void DeleteStation(int stationId,string userId);
        TBL_STATION GetStationDetailById(int stationId);
    }

    public  class StationService : IStationService
    {
        private static   IStationRepository stationRepository;
        private readonly IUnitOfWork unitOfWork;

        public StationService(IStationRepository _stationRepository, IUnitOfWork unitOfWork)
        {
           stationRepository = _stationRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IStationService Members

        public static IQueryable<TBL_STATION> GetStationsAsQuery()
        {
                return stationRepository.GetAllQuery(x => x.IS_DELETED == false);           
        }

        public List<TBL_STATION> GetStations()
        {
            return stationRepository.GetAll();
        }

        public List<TBL_STATION> GetStations(Expression<Func<TBL_STATION, bool>> where)
        {
            return stationRepository.GetMany(where);
        }

        public List<TBL_STATION> GetStationByCompanyId(int companyId)
        {
            return stationRepository.GetMany(x => x.COMPANY_ID == companyId && x.IS_DELETED == false).ToList();
        }


        public bool IsStationExist(TBL_STATION station)
        {            
            return stationRepository.Get(x => x.IS_DELETED == false && x.ID != station.ID && station.COMPANY_ID == x.COMPANY_ID && x.NAME.ToUpper() == station.NAME.ToUpper()) == null ? false : true;
        }

        public void UpdateStation(TBL_STATION station)
        {
            stationRepository.Update(station);
        }

        //public void UpdateStation(Expression<Func<TBL_STATION, bool>> where, Expression<Func<TBL_STATION, TBL_STATION>> updateValues)
        //{
        //    stationRepository.UpdateBulk(where, updateValues);
        //}

        public void DeleteStation(int stationId, string userId)
        {
            TBL_STATION comp = stationRepository.GetById(stationId);
            comp.UPDATE_USER = userId;
            comp.IS_DELETED = true;
            stationRepository.Update(comp);
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

        public void CreateStation(TBL_STATION station)
        {
            stationRepository.Add(station);
        }

        public void SaveStation()
        {
            unitOfWork.Commit();
        }

        public TBL_STATION GetStationDetailById(int stationId)
        {
            return stationRepository.GetMany(s => s.ID == stationId).FirstOrDefault();
        }

        #endregion
    }
}
