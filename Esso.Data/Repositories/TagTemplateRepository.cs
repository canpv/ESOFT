using Esso.Data.Infrastructure;
using Esso.Models;
using System;

namespace Esso.Data.Repositories
{
    public class TagTemplateRepository : RepositoryBase<TBL_TAG_TEMP>, ITagTemplateRepository
    {
        public TagTemplateRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
     
        public override void Update(TBL_TAG_TEMP entity)
        {
            entity.UPDATED_DATE = DateTime.Now;
            base.Update(entity);
        }
    }

    public interface ITagTemplateRepository : IRepository<TBL_TAG_TEMP>
    {
    }
}
