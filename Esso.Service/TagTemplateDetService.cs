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
    public interface ITagTemplateDetService
    {
        List<TBL_TAG_TEMP_DET> GetTagTemplateDets(Expression<Func<TBL_TAG_TEMP_DET, bool>> where);
        void CreateTagTemplateDet(TBL_TAG_TEMP_DET tagTemplateDet);
        void DeleteTagTemplateDet(int tagTemplateDetId, string userId);
        void UpdateTagTemplateDet(TBL_TAG_TEMP_DET tagTemplateDet);
        //void UpdateTagTemplateDet(Expression<Func<TBL_TAG_TEMP_DET, bool>> where, Expression<Func<TBL_TAG_TEMP_DET, TBL_TAG_TEMP_DET>> updateValues);
        void SaveTagTemplateDet();

    }

    public  class TagTemplateDetService : ITagTemplateDetService
    {
        private static   ITagTemplateDetRepository tagTemplateDetRepository;
        private readonly IUnitOfWork unitOfWork;

        public TagTemplateDetService(ITagTemplateDetRepository _tagTemplateDetRepository, IUnitOfWork unitOfWork)
        {
           tagTemplateDetRepository = _tagTemplateDetRepository;
            this.unitOfWork = unitOfWork;
        }

        #region ITagTemplateDetService Members

    

        public static IQueryable<TBL_TAG_TEMP_DET> GetTagTemplateDetsAsQuery(Expression<Func<TBL_TAG_TEMP_DET, bool>> where)
        {
            return tagTemplateDetRepository.GetAllQuery(where);
        }

        public List<TBL_TAG_TEMP_DET> GetTagTemplateDets(Expression<Func<TBL_TAG_TEMP_DET, bool>> where)
        {
            return tagTemplateDetRepository.GetMany(where).ToList();
        }

        public void UpdateTagTemplateDet(TBL_TAG_TEMP_DET tagTemplateDet)
        {
            tagTemplateDetRepository.Update(tagTemplateDet);
        }

        //public void UpdateTagTemplateDet(Expression<Func<TBL_TAG_TEMP_DET, bool>> where, Expression<Func<TBL_TAG_TEMP_DET, TBL_TAG_TEMP_DET>> updateValues)
        //{
        //    tagTemplateDetRepository.UpdateBulk(where, updateValues);
        //}

        public void DeleteTagTemplateDet(int tagTemplateDetId, string userId)
        {
            TBL_TAG_TEMP_DET comp = tagTemplateDetRepository.GetById(tagTemplateDetId);
            comp.UPDATE_USER = userId;
            comp.IS_DELETED = true;
            tagTemplateDetRepository.Update(comp);
        }

        public void CreateTagTemplateDet(TBL_TAG_TEMP_DET tagTemplateDet)
        {
            tagTemplateDetRepository.Add(tagTemplateDet);
        }

        public void SaveTagTemplateDet()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
