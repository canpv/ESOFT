using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        EssoEntities dbContext;

        public EssoEntities Init()
        {
            return dbContext ?? (dbContext = new EssoEntities());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}
