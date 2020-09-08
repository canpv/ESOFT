using Esso.Data.Infrastructure;
using Esso.Models;
using System;

namespace Esso.Data.Repositories
{
    public class StationUserRepository : RepositoryBase<TBL_STATION_USER>, IStationUserRepository
    {
        public StationUserRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

     
        public override void Update(TBL_STATION_USER entity)
        {
            entity.UPDATED_DATE = DateTime.Now;
            base.Update(entity);
        }
    }

    public interface IStationUserRepository : IRepository<TBL_STATION_USER>
    {
    }
}
