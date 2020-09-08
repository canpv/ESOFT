using Esso.Data.Infrastructure;
using Esso.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Data.Repositories
{
    public class InvOzetRepository: RepositoryBase<TBL_INV_OZET>, IInvOzetRepository
    {
        public InvOzetRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        //public IQueryable<TBL_OZET> GetProductionGraphic()
        //{
        //    return null;
        //}

    }

    public interface IInvOzetRepository : IRepository<TBL_INV_OZET>
    {
        //IEnumerable<Company> GetCompanies();
    }

}
