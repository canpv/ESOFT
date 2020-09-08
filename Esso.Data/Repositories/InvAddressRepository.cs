using Esso.Data.Infrastructure;
using Esso.Models;
using System;

namespace Esso.Data.Repositories
{
    public class InvAddressRepository : RepositoryBase<TBL_INV_ADDRESS>, IInvAddressRepository
    {
        public InvAddressRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

     
        public override void Update(TBL_INV_ADDRESS entity)
        {
            entity.UPDATED_DATE = DateTime.Now;
            base.Update(entity);
        }

    }

    public interface IInvAddressRepository : IRepository<TBL_INV_ADDRESS>
    {
    }
}
