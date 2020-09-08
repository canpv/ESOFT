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
    public interface ICompanyService
    {
        List<TBL_COMPANY> GetCompanies();
        List<TBL_COMPANY> GetCompanies(Expression<Func<TBL_COMPANY, bool>> where);
        void CreateCompany(TBL_COMPANY company);
        void SaveCompany();
        bool IsCompanyExist(TBL_COMPANY company);
        void UpdateCompany(TBL_COMPANY company);
        void DeleteCompany(int companyId,string userId);
    }

    public  class CompanyService : ICompanyService
    {
        private static   ICompanyRepository companyRepository;
        private readonly IUnitOfWork unitOfWork;

        public CompanyService(ICompanyRepository _companyRepository, IUnitOfWork unitOfWork)
        {
           companyRepository = _companyRepository;
            this.unitOfWork = unitOfWork;
        }

        #region ICategoryService Members

        public static IQueryable<TBL_COMPANY> GetCompaniesAsQuery()
        {
                return companyRepository.GetAllQuery(x => x.IS_DELETED == false);           
        }

        public List<TBL_COMPANY> GetCompanies()
        {
            return companyRepository.GetAll();
        }

        public List<TBL_COMPANY> GetCompanies(Expression<Func<TBL_COMPANY, bool>> where)
        {
            return companyRepository.GetMany(where).ToList();
        }

        public bool IsCompanyExist(TBL_COMPANY company)
        {
            return companyRepository.Get(x => x.IS_DELETED == false && x.ID != company.ID && x.NAME.ToUpper() == company.NAME.ToUpper()) == null ? false : true;
        }

        public void UpdateCompany(TBL_COMPANY company)
        {
            companyRepository.Update(company);
        }



        public void DeleteCompany(int companyId,string userId)
        {
            TBL_COMPANY comp = companyRepository.GetById(companyId);
            comp.UPDATE_USER = userId;
            comp.IS_DELETED = true;
            companyRepository.Update(comp);
        }

        public void CreateCompany(TBL_COMPANY company)
        {
            companyRepository.Add(company);
        }

        public void SaveCompany()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
