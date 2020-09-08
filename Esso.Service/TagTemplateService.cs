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
    public interface ITagTemplateService
    {
        bool IsTagTemplateExist(TBL_TAG_TEMP station);
        List<TBL_TAG_TEMP> GetTagTemplates(Expression<Func<TBL_TAG_TEMP, bool>> where);
        void CreateTagTemplate(TBL_TAG_TEMP tagTemplate);
        void DeleteTagTemplate(int tagTemplateId, string userId);
        void UpdateTagTemplate(TBL_TAG_TEMP tagTemplate);
        //void UpdateTagTemplate(Expression<Func<TBL_TAG_TEMP, bool>> where, Expression<Func<TBL_TAG_TEMP, TBL_TAG_TEMP>> updateValues);
        void SaveTagTemplate();
    }

    public  class TagTemplateService : ITagTemplateService
    {
        private static   ITagTemplateRepository tagTemplateRepository;
        private readonly IUnitOfWork unitOfWork;

        public TagTemplateService(ITagTemplateRepository _tagTemplateRepository, IUnitOfWork unitOfWork)
        {
           tagTemplateRepository = _tagTemplateRepository;
            this.unitOfWork = unitOfWork;
        }

        #region ITagTemplateService Members

        public bool IsTagTemplateExist(TBL_TAG_TEMP tagTemplate)
        {
            return tagTemplateRepository.Get(x => x.COMPANY_ID == tagTemplate.COMPANY_ID && x.IS_DELETED == false && x.ID != tagTemplate.ID && x.NAME.ToUpper() == tagTemplate.NAME.ToUpper()) == null ? false : true;
        }

        public static IQueryable<TBL_TAG_TEMP> GetTagTemplatesAsQuery(Expression<Func<TBL_TAG_TEMP, bool>> where)
        {
            return tagTemplateRepository.GetAllQuery(where);
        }

        public List<TBL_TAG_TEMP> GetTagTemplates(Expression<Func<TBL_TAG_TEMP, bool>> where)
        {
            return tagTemplateRepository.GetMany(where);
        }

        public void UpdateTagTemplate(TBL_TAG_TEMP tagTemplate)
        {
            tagTemplateRepository.Update(tagTemplate);
        }

        //public void UpdateTagTemplate(Expression<Func<TBL_TAG_TEMP, bool>> where, Expression<Func<TBL_TAG_TEMP, TBL_TAG_TEMP>> updateValues)
        //{
        //    tagTemplateRepository.UpdateBulk(where, updateValues);
        //}

        public void DeleteTagTemplate(int tagTemplateId, string userId)
        {
            TBL_TAG_TEMP comp = tagTemplateRepository.GetById(tagTemplateId);
            comp.UPDATE_USER = userId;
            comp.IS_DELETED = true;
            tagTemplateRepository.Update(comp);
        }

        public void CreateTagTemplate(TBL_TAG_TEMP tagTemplate)
        {
            tagTemplateRepository.Add(tagTemplate);
        }

        public void SaveTagTemplate()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
