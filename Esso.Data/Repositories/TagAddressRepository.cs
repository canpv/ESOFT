using Esso.Data.Infrastructure;
using Esso.Models;
using System;

namespace Esso.Data.Repositories
{
    public class TagAddressRepository : RepositoryBase<TBL_TAG_TEMP_DET>, ITagAddressRepository
    {
        public TagAddressRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

     
        public override void Update(TBL_TAG_TEMP_DET entity)
        {
            entity.UPDATED_DATE = DateTime.Now;
            base.Update(entity);
        }
    }

    public interface ITagAddressRepository : IRepository<TBL_TAG_TEMP_DET>
    {
    }
}
