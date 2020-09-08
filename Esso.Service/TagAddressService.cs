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
    public interface ITagAddressService
    {
        //bool IsTagAddressExist(TBL_TAG_TEMP_DET station);
        IEnumerable<TBL_TAG_TEMP_DET> GetTagAddresss(Expression<Func<TBL_TAG_TEMP_DET, bool>> where);
        void CreateTagAddress(TBL_TAG_TEMP_DET tagAddress);
        void DeleteTagAddress(int tagAddressId, string userId);
        void UpdateTagAddress(TBL_TAG_TEMP_DET tagAddress);
        void SaveTagAddress();
    }

    public  class TagAddressService : ITagAddressService
    {
        private static   ITagAddressRepository tagAddressRepository;
        private readonly IUnitOfWork unitOfWork;

        public TagAddressService(ITagAddressRepository _tagAddressRepository, IUnitOfWork unitOfWork)
        {
           tagAddressRepository = _tagAddressRepository;
            this.unitOfWork = unitOfWork;
        }

        #region ITagAddressService Members

        //public bool IsTagAddressExist(TBL_TAG_TEMP_DET tagAddress)
        //{
        //    return tagAddressRepository.Get(x => x.STATION_ID == tagAddress.STATION_ID && x.IS_DELETED == false && x.ID != tagAddress.ID && x.NAME.ToUpper() == tagAddress.NAME.ToUpper()) == null ? false : true;
        //}

        public static IQueryable<TBL_TAG_TEMP_DET> GetTagAddresssAsQuery(Expression<Func<TBL_TAG_TEMP_DET, bool>> where)
        {
            return tagAddressRepository.GetAllQuery(where);
        }

        public IEnumerable<TBL_TAG_TEMP_DET> GetTagAddresss(Expression<Func<TBL_TAG_TEMP_DET, bool>> where)
        {
            return tagAddressRepository.GetMany(where);
        }

        public void UpdateTagAddress(TBL_TAG_TEMP_DET tagAddress)
        {
            tagAddressRepository.Update(tagAddress);
        }

        public void UpdateTagAddress(Expression<Func<TBL_TAG_TEMP_DET, bool>> where, Expression<Func<TBL_TAG_TEMP_DET, TBL_TAG_TEMP_DET>> updateValues)
        {
            tagAddressRepository.UpdateBulk(where, updateValues);
        }

        public void DeleteTagAddress(int tagAddressId, string userId)
        {
            TBL_TAG_TEMP_DET comp = tagAddressRepository.GetById(tagAddressId);
            comp.UPDATE_USER = userId;
            comp.IS_DELETED = true;
            tagAddressRepository.Update(comp);
        }

        public void CreateTagAddress(TBL_TAG_TEMP_DET tagAddress)
        {
            tagAddressRepository.Add(tagAddress);
        }

        public void SaveTagAddress()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
