using Esso.Data.Infrastructure;
using Esso.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Data.Repositories
{
    public class GraphicRepository : RepositoryBase<TBL_OZET>, IGraphicRepository
    {
        public GraphicRepository(IDbFactory dbFactory)
            : base(dbFactory) {    }

        //public IQueryable<TBL_OZET> GetProductionGraphic()
        //{
        //    return null;
        //}

    }

    public interface IGraphicRepository : IRepository<TBL_OZET>
    {
        //IEnumerable<Company> GetCompanies();
    }
}
