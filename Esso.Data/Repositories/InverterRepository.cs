using Esso.Data.Infrastructure;
using Esso.Models;
using System;

namespace Esso.Data.Repositories
{
    public class InverterRepository : RepositoryBase<TBL_INVERTER>, IInverterRepository
    {
        public InverterRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

     
        public override void Update(TBL_INVERTER entity)
        {
            entity.UPDATED_DATE = DateTime.Now;
            base.Update(entity);
        }
    }

    public interface IInverterRepository : IRepository<TBL_INVERTER>
    {
    }
}
