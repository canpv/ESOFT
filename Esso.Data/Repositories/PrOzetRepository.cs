using Esso.Data.Infrastructure;
using Esso.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Data.Repositories
{
    public class PrOzetRepository:RepositoryBase<TBL_PR_OZET>, IPrOzetRepository
    {
        public PrOzetRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        //public IQueryable<TBL_OZET> GetProductionGraphic()
        //{
        //    return null;
        //}

    }

    public interface IPrOzetRepository : IRepository<TBL_PR_OZET>
    {
        //IEnumerable<Company> GetCompanies();
    }
}