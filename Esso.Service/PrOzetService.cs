using Esso.Data.Infrastructure;
using Esso.Data.Repositories;
using Esso.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Esso.Service
{
    public interface IPrOzetService
    {
        List<TBL_PR_OZET> GetPrOzet();
        List<TBL_PR_OZET> GetPrOzet(Expression<Func<TBL_PR_OZET, bool>> where); 
    }

    public class PrOzetService : IPrOzetService
    {
        private static IPrOzetRepository prOzetRepository;
        private readonly IUnitOfWork unitOfWork;

        public PrOzetService(IPrOzetRepository _prOzetRepository, IUnitOfWork unitOfWork)
        {
            prOzetRepository = _prOzetRepository;
            this.unitOfWork = unitOfWork;
        }

        public List<TBL_PR_OZET> GetPrOzet()
        {
            List<TBL_PR_OZET> q = prOzetRepository.GetAll();
            return q;
        }

        public List<TBL_PR_OZET> GetPrOzet(Expression<Func<TBL_PR_OZET, bool>> where)
        {
            return prOzetRepository.GetMany(where);
        }
    }
}
