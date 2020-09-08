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
    public interface IInvOzetService
    {
        IEnumerable<TBL_INV_OZET> GetInvOzet(Expression<Func<TBL_INV_OZET, bool>> where);
        IEnumerable<TBL_INV_OZET> GetInvOzet();
    }

    public class InvOzetService : IInvOzetService
    {
        private static IInvOzetRepository invOzetRepository;
        private readonly IUnitOfWork unitOfWork;

        public InvOzetService(IInvOzetRepository _invOzetRepository, IUnitOfWork unitOfWork)
        {
            invOzetRepository = _invOzetRepository;
            this.unitOfWork = unitOfWork;
        }

        public IEnumerable<TBL_INV_OZET> GetInvOzet(Expression<Func<TBL_INV_OZET, bool>> where)
        {
            return invOzetRepository.GetMany(where);
        }

        public IEnumerable<TBL_INV_OZET> GetInvOzet()
        {
            IEnumerable<TBL_INV_OZET> q = invOzetRepository.GetAll();
            return q;
        }
    }
}
