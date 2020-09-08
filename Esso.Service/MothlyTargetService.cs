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
    public interface IMonthlyTargetService
    {
        IEnumerable<TBL_MONTHLY_TARGET> GetMTarget(Expression<Func<TBL_MONTHLY_TARGET, bool>> where);
        IEnumerable<TBL_MONTHLY_TARGET> GetMTarget();
        List<TBL_MONTHLY_TARGET> GetMonthlyTarget(Expression<Func<TBL_MONTHLY_TARGET, bool>> where);
        //TBL_MONTHLY_TARGET GetAATek(Expression<Func<TBL_MONTHLY_TARGET, bool>> where);
    }
    public class MothlyTargetService : IMonthlyTargetService
    {
        private static IMonthlyTargetRepository monthlyTargetRepository;
        private readonly IUnitOfWork unitOfWork;

        public MothlyTargetService(IMonthlyTargetRepository _monthlyTargetRepository, IUnitOfWork unitOfWork)
        {
            monthlyTargetRepository = _monthlyTargetRepository;
            this.unitOfWork = unitOfWork;
        }

        //public TBL_MONTHLY_TARGET GetAATek(Expression<Func<TBL_MONTHLY_TARGET, bool>> where)
        //{
        //    //return monthlyTargetRepository.GetByDate(where);
        //}

        public List<TBL_MONTHLY_TARGET> GetMonthlyTarget(Expression<Func<TBL_MONTHLY_TARGET, bool>> where)
        {
            return monthlyTargetRepository.GetMany(where);
        }


        public IEnumerable<TBL_MONTHLY_TARGET> GetMTarget(Expression<Func<TBL_MONTHLY_TARGET, bool>> where)
        {
            return monthlyTargetRepository.GetMany(where);
        }

        public IEnumerable<TBL_MONTHLY_TARGET> GetMTarget()
        {
            IEnumerable<TBL_MONTHLY_TARGET> q = monthlyTargetRepository.GetAll();

            return q;
        }
    }
}
