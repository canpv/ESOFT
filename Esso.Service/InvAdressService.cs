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
    public interface IInvAddressService
    {
        List<TBL_INV_ADDRESS> GetInvAddresss();
        List<TBL_INV_ADDRESS> GetInvAddresss(Expression<Func<TBL_INV_ADDRESS, bool>> where);

        void CreateInvAddress(TBL_INV_ADDRESS station);
        void SaveInvAddress();
        List<TBL_INV_ADDRESS> GetInvAddressByInvId(int inverterId);
        //bool IsInvAddressExist(TBL_INV_ADDRESS station);
        void UpdateInvAddress(TBL_INV_ADDRESS station);
        void UpdateInvAddress(Expression<Func<TBL_INV_ADDRESS, bool>> where, Expression<Func<TBL_INV_ADDRESS, TBL_INV_ADDRESS>> updateValues);
        void DeleteInvAddress(int stationId, string userId);
    }

    public class InvAddressService : IInvAddressService
    {
        private static IInvAddressRepository stationRepository;
        private readonly IUnitOfWork unitOfWork;

        public InvAddressService(IInvAddressRepository _stationRepository, IUnitOfWork unitOfWork)
        {
            stationRepository = _stationRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IInvAddressService Members

        public static IQueryable<TBL_INV_ADDRESS> GetInvAddresssAsQuery()
        {
            return stationRepository.GetAllQuery(x => x.IS_DELETED == 0);
        }

        public List<TBL_INV_ADDRESS> GetInvAddresss()
        {
            return stationRepository.GetAll();
        }

        public List<TBL_INV_ADDRESS> GetInvAddresss(Expression<Func<TBL_INV_ADDRESS, bool>> where)
        {
            return stationRepository.GetMany(where);
        }

        public List<TBL_INV_ADDRESS> GetInvAddressByInvId(int inverterId)
        {
            return stationRepository.GetMany(x => x.INV_ID == inverterId && x.IS_DELETED == 0).ToList();
        }


        //public bool IsInvAddressExist(TBL_INV_ADDRESS station)
        //{
        //    return stationRepository.Get(x => x.IS_DELETED == false && x.ID != station.ID && station.COMPANY_ID == x.COMPANY_ID && x.NAME.ToUpper() == station.NAME.ToUpper()) == null ? false : true;
        //}

        public void UpdateInvAddress(TBL_INV_ADDRESS station)
        {
            stationRepository.Update(station);
        }

        public void UpdateInvAddress(Expression<Func<TBL_INV_ADDRESS, bool>> where, Expression<Func<TBL_INV_ADDRESS, TBL_INV_ADDRESS>> updateValues)
        {
            stationRepository.UpdateBulk(where, updateValues);
        }

        public void DeleteInvAddress(int stationId, string userId)
        {
            TBL_INV_ADDRESS comp = stationRepository.GetById(stationId);
            comp.UPDATE_USER = userId;
            comp.IS_DELETED = 1;
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

        public void CreateInvAddress(TBL_INV_ADDRESS station)
        {
            stationRepository.Add(station);
        }

        public void SaveInvAddress()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
