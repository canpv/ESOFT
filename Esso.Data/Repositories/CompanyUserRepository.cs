using Esso.Data.Infrastructure;
using Esso.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Data.Repositories
{
    public class CompanyUserRepository : RepositoryBase<TBL_COMPANY_USER>, ICompanyUserRepository
    {
        public CompanyUserRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

     
        public override void Update(TBL_COMPANY_USER entity)
        {
            entity.UPDATED_DATE = DateTime.Now;
            base.Update(entity);
        }
    }

    public interface ICompanyUserRepository : IRepository<TBL_COMPANY_USER>
    {
    }
}
