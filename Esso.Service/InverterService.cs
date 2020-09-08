using Esso.Data.Infrastructure;
using Esso.Data.Repositories;
using Esso.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Esso.Service
{
    // operations you want to expose
    public interface IInverterService
    {
        bool IsInverterExist(TBL_INVERTER station);
        List<TBL_INVERTER> GetInverters(Expression<Func<TBL_INVERTER, bool>> where);
        void CreateInverter(TBL_INVERTER inverter);
        void DeleteInverter(int inverterId, string userId);
        void UpdateInverter(TBL_INVERTER inverter);
        void SaveInverter();
    }

    public  class InverterService : IInverterService
    {
        private static   IInverterRepository inverterRepository;
        private readonly IUnitOfWork unitOfWork;

        public InverterService(IInverterRepository _inverterRepository, IUnitOfWork unitOfWork)
        {
           inverterRepository = _inverterRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IInverterService Members

        public bool IsInverterExist(TBL_INVERTER inverter)
        {
            return inverterRepository.Get(x => x.STATION_ID == inverter.STATION_ID && x.IS_DELETED == false && x.ID != inverter.ID && x.NAME.ToUpper() == inverter.NAME.ToUpper()) == null ? false : true;
        }

        public static IQueryable<TBL_INVERTER> GetInvertersAsQuery(Expression<Func<TBL_INVERTER, bool>> where)
        {
            return inverterRepository.GetAllQuery(where);
        }

        public List<TBL_INVERTER> GetInverters(Expression<Func<TBL_INVERTER, bool>> where)
        {
            return inverterRepository.GetMany(where);
        }

        public void UpdateInverter(TBL_INVERTER inverter)
        {
            inverterRepository.Update(inverter);
        }

        public void DeleteInverter(int inverterId, string userId)
        {
            TBL_INVERTER comp = inverterRepository.GetById(inverterId);
            comp.UPDATE_USER = userId;
            comp.IS_DELETED = true;
            inverterRepository.Update(comp);
        }

        public void CreateInverter(TBL_INVERTER inverter)
        {
            inverterRepository.Add(inverter);
        }

        public void SaveInverter()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
