using Esso.Data.Infrastructure;
using Esso.Data.Repositories;
using Esso.Models;
using Esso.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Esso.Service
{
    // operations you want to expose
    public interface ITagService
    {
        bool IsTagExist(TBL_TAG station);
        List<TBL_TAG> GetTags(Expression<Func<TBL_TAG, bool>> where);
        void CreateTag(TBL_TAG Tag);
        void DeleteTag(int TagId, string userId);
        void UpdateTag(TBL_TAG Tag);
        void SaveTag();
    }

    public class TagService : ITagService
    {
        private static ITagRepository TagRepository;
        private readonly IUnitOfWork unitOfWork;

        public TagService(ITagRepository _TagRepository, IUnitOfWork unitOfWork)
        {
            TagRepository = _TagRepository;
            this.unitOfWork = unitOfWork;
        }

        #region ITagService Members

        public bool IsTagExist(TBL_TAG Tag)
        {
            return TagRepository.Get(x => x.IS_DELETED == false && x.NAME.ToUpper() == Tag.NAME.ToUpper()) == null ? false : true;
        }

        public static IQueryable<TBL_TAG> GetTagsAsQuery(Expression<Func<TBL_TAG, bool>> where)
        {
            return TagRepository.GetAllQuery(where);
        }

        public List<TBL_TAG> GetTags(Expression<Func<TBL_TAG, bool>> where)
        {
            return TagRepository.GetMany(where);
        }

        public void UpdateTag(TBL_TAG Tag)
        {
            TagRepository.Update(Tag);
        }

        public void DeleteTag(int TagId, string userId)
        {
            TBL_TAG comp = TagRepository.GetById(TagId);
            comp.UPDATE_USER = userId;
            comp.IS_DELETED = true;
            TagRepository.Update(comp);
        }

        public void CreateTag(TBL_TAG Tag)
        {
            TagRepository.Add(Tag);
        }

        public void SaveTag()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
