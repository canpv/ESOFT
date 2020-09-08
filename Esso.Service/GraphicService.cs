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
    public interface IGraphicService
    {
        List<TBL_OZET> GetProductionGraphic(Expression<Func<TBL_OZET, bool>> where);
        List<TBL_OZET> GetGraphicData();
    }

    public class GraphicService : IGraphicService
    {
        private static IGraphicRepository graphicRepository;
        private readonly IUnitOfWork unitOfWork;

        public GraphicService(IGraphicRepository _graphicRepository, IUnitOfWork unitOfWork)
        {
            graphicRepository = _graphicRepository;
            this.unitOfWork = unitOfWork;
        }

        public List<TBL_OZET> GetGraphicData()
        {

           return  graphicRepository.GetAll();

            
        }
       

        public List<TBL_OZET> GetProductionGraphic(Expression<Func<TBL_OZET, bool>> where)
        {
            return graphicRepository.GetMany(where);
        }


    }
}
