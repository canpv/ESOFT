using Esso.Data.Infrastructure;
using Esso.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Data.Repositories
{
    public class StationGroupRepository : RepositoryBase<TBL_STATION_GROUP>, IStationGroupRepository
    {
        public StationGroupRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        //public IQueryable<StationGroup> GetAllQuery()
        //{
        //    return  from station in DbFactory..
        //}

        public override void Update(TBL_STATION_GROUP entity)
        {
            entity.UPDATED_DATE = DateTime.Now;
            base.Update(entity);
        }
    }

    public interface IStationGroupRepository : IRepository<TBL_STATION_GROUP>
    {
    }
}
