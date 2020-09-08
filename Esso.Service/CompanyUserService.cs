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
    // operations you want to expose
    public interface ICompanyUserService
    {
        List<TBL_COMPANY_USER> GetCompanyUsers(Expression<Func<TBL_COMPANY_USER, bool>> where);
        void CreateCompanyUser(TBL_COMPANY_USER companyUser);
        void SaveCompanyUser();
        void UpdateCompanyUser(TBL_COMPANY_USER companyUser);
        void DeleteCompanyUser(int companyUserId);
    }

    public  class CompanyUserService : ICompanyUserService
    {
        private static   ICompanyUserRepository companyUserRepository;
        private readonly IUnitOfWork unitOfWork;

        public CompanyUserService(ICompanyUserRepository _companyUserRepository, IUnitOfWork unitOfWork)
        {
           companyUserRepository = _companyUserRepository;
            this.unitOfWork = unitOfWork;
        }

        #region ICompanyUserService Members

        public List<TBL_COMPANY_USER> GetCompanyUsers(Expression<Func<TBL_COMPANY_USER, bool>> where)
        {
            return companyUserRepository.GetMany(where);
        }


        public void UpdateCompanyUser(TBL_COMPANY_USER companyUser)
        {
            companyUserRepository.Update(companyUser);
        }

        public void DeleteCompanyUser(int companyUserId)
        {
            TBL_COMPANY_USER comp = companyUserRepository.GetById(companyUserId);
            comp.IS_DELETED = true;
            companyUserRepository.Update(comp);
        }


        public void CreateCompanyUser(TBL_COMPANY_USER companyUser)
        {
            companyUserRepository.Add(companyUser);
        }

        public void SaveCompanyUser()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
