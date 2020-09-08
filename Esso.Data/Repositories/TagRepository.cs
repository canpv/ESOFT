using Esso.Data.Infrastructure;
using Esso.Models;
using Esso.Models;
using System;

namespace Esso.Data.Repositories
{
    public class TagRepository : RepositoryBase<TBL_TAG>, ITagRepository
    {
        public TagRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

     
        public override void Update(TBL_TAG entity)
        {
            entity.UPDATED_DATE = DateTime.Now;
            base.Update(entity);
        }
    }

    public interface ITagRepository : IRepository<TBL_TAG>
    {
    }
}
