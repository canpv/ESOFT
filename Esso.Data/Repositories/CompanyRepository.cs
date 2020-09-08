using Esso.Data.Infrastructure;
using Esso.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Data.Repositories
{
    public class CompanyRepository : RepositoryBase<TBL_COMPANY>, ICompanyRepository
    {
        public CompanyRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        //public IQueryable<Company> GetAllQuery()
        //{
        //    return  from company in DbFactory..


        //}

        public override void Update(TBL_COMPANY entity)
        {
            entity.UPDATED_DATE = DateTime.Now;
            base.Update(entity);
        }

    }

    public interface ICompanyRepository : IRepository<TBL_COMPANY>
    {
    }
}
