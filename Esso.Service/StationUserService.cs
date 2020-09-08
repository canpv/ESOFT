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
    public interface IStationUserService
    {
        List<TBL_STATION_USER> GetStationUsers(Expression<Func<TBL_STATION_USER, bool>> where);
        void CreateStationUser(TBL_STATION_USER stationUser);
        void SaveStationUser();
        void UpdateStationUser(TBL_STATION_USER stationUser);
        //void UpdateStationUser(Expression<Func<TBL_STATION_USER, bool>> where, Expression<Func<TBL_STATION_USER, TBL_STATION_USER>> updateValues);
        void DeleteStationUser(int stationUserId);
    }

    public  class StationUserService : IStationUserService
    {
        private static   IStationUserRepository stationUserRepository;
        private readonly IUnitOfWork unitOfWork;

        public StationUserService(IStationUserRepository _stationUserRepository, IUnitOfWork unitOfWork)
        {
           stationUserRepository = _stationUserRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IStationUserService Members

        public List<TBL_STATION_USER> GetStationUsers(Expression<Func<TBL_STATION_USER, bool>> where)
        {
            return stationUserRepository.GetMany(where);
        }

        public void UpdateStationUser(TBL_STATION_USER stationUser)
        {
            stationUserRepository.Update(stationUser);
        }

        //public void UpdateStationUser(Expression<Func<TBL_STATION_USER, bool>> where, Expression<Func<TBL_STATION_USER, TBL_STATION_USER>> updateValues)
        //{
        //    stationUserRepository.UpdateBulk(where, updateValues);
        //}

        public void DeleteStationUser(int stationUserId)
        {
            TBL_STATION_USER comp = stationUserRepository.GetById(stationUserId);
            comp.IS_DELETED = true;
            stationUserRepository.Update(comp);
        }


        public void CreateStationUser(TBL_STATION_USER stationUser)
        {
            stationUserRepository.Add(stationUser);
        }

        public void SaveStationUser()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
