using Esso.Data.Infrastructure;
using Esso.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Data.Repositories
{
    public class MonthlyTargetRepository : RepositoryBase<TBL_MONTHLY_TARGET>, IMonthlyTargetRepository
    {
        public MonthlyTargetRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        //public IQueryable<TBL_OZET> GetProductionGraphic()
        //{
        //    return null;
        //}

    }

    public interface IMonthlyTargetRepository : IRepository<TBL_MONTHLY_TARGET>
    {
        //IEnumerable<Company> GetCompanies();
    }
}
