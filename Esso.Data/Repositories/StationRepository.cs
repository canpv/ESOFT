using Esso.Data.Infrastructure;
using Esso.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Data.Repositories
{
    public class StationRepository : RepositoryBase<TBL_STATION>, IStationRepository
    {
        public StationRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
        
        public override void Update(TBL_STATION entity)
        {
            entity.UPDATED_DATE = DateTime.Now;
            base.Update(entity);
        }
    }

    public interface IStationRepository : IRepository<TBL_STATION>
    {
     
    }
}
