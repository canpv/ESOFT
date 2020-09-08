using Esso.Data.Infrastructure;
using Esso.Models;
using System;

namespace Esso.Data.Repositories
{
    public class AlarmDefinitionRepository : RepositoryBase<TBL_ALARM_DEF>, IAlarmDefinitionRepository
    {
        public AlarmDefinitionRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

     
        public override void Update(TBL_ALARM_DEF entity)
        {
            entity.UPDATED_DATE = DateTime.Now;
            base.Update(entity);
        }
    }

    public interface IAlarmDefinitionRepository : IRepository<TBL_ALARM_DEF>
    {
    }
}
