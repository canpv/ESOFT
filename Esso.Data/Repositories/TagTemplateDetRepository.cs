using Esso.Data.Infrastructure;
using Esso.Models;
using System;

namespace Esso.Data.Repositories
{
    public class TagTemplateDetRepository : RepositoryBase<TBL_TAG_TEMP_DET>, ITagTemplateDetRepository
    {
        public TagTemplateDetRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

     
        public override void Update(TBL_TAG_TEMP_DET entity)
        {
            entity.UPDATED_DATE = DateTime.Now;
            base.Update(entity);
        }
    }

    public interface ITagTemplateDetRepository : IRepository<TBL_TAG_TEMP_DET>
    {
    }
}
