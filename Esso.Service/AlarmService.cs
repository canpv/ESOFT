using Esso.Data.Infrastructure;
using Esso.Data.Repositories;
using Esso.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Esso.Service
{
    // operations you want to expose
    public interface IAlarmDefinitionService
    {
        //List<TBL_ALARM_LOG> GetAlarmDefinitions(Expression<Func<TBL_ALARM_DEF, bool>> where);
        //static IQueryable<TBL_ALARM_LOG> GetAlarmsAsQuery(Expression<Func<TBL_ALARM_LOG, bool>> where);
        void CreateAlarmDefinition(TBL_ALARM_DEF Alarm);
        void DeleteAlarmDefinition(int Id, string userId);
        void UpdateAlarmDefinition(TBL_ALARM_DEF Alarm);
        void SaveAlarmDefinition();
    }

    public class AlarmDefinitionService : IAlarmDefinitionService
    {
        private static IAlarmDefinitionRepository AlarmDefinitionRepository;
        private readonly IUnitOfWork unitOfWork;

        public AlarmDefinitionService(IAlarmDefinitionRepository _AlarmDefinitionRepository, IUnitOfWork unitOfWork)
        {
            AlarmDefinitionRepository = _AlarmDefinitionRepository;
            this.unitOfWork = unitOfWork;
        }

        #region IAlarmService Members

        //public bool IsAlarmExist(TBL_ALARM_LOG Alarm)
        //{
        //    return AlarmRepository.Get(x => x.IS_DELETED == false && x.NAME.ToUpper() == Alarm.NAME.ToUpper()) == null ? false : true;
        //}

        //public static IQueryable<TBL_ALARM_LOG> GetAlarmsAsQuery(Expression<Func<TBL_ALARM_LOG, bool>> where)
        //{
        //    return AlarmRepository.GetAllQuery(where);
        //}

        //public List<TBL_ALARM_LOG> GetAlarms(Expression<Func<TBL_ALARM_LOG, bool>> where)
        //{
        //    return AlarmRepository.GetMany(where);
        //}

        public void UpdateAlarmDefinition(TBL_ALARM_DEF Alarm)
        {
            AlarmDefinitionRepository.Update(Alarm);
        }

        public void DeleteAlarmDefinition(int AlarmId, string userId)
        {
            TBL_ALARM_DEF comp = AlarmDefinitionRepository.GetById(AlarmId);
            comp.UPDATE_USER = userId;
            comp.IS_DELETED = true;
            AlarmDefinitionRepository.Update(comp);
        }

        public void CreateAlarmDefinition(TBL_ALARM_DEF Alarm)
        {
            AlarmDefinitionRepository.Add(Alarm);
        }

        public void SaveAlarmDefinition()
        {
            unitOfWork.Commit();
        }

        #endregion
    }
}
