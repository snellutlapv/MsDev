using System;
using Nancy;
using Nancy.Responses;
using PV.App.Managers.Standard.Helpers;
using PV.Data.Standard;
using PV.Data.Standard.EntityClasses;
using PV.Data.Standard.HelperClasses;
using PV.Data.Standard.TypedViewClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace MsLogDetail
{
    public class LogDetailModel
    {
        public LogDetailModel(LogDetailEntity entity)
        {
            LogNum = entity.LogNum;
            Practice = entity.Practice;
            Clinic = entity.Clinic;
            PatNum = entity.PatNum;
            SvcDate = entity.SvcDate;
            VisitType = entity.VisitType;
            CmpNum = entity.CmpNum;
            Protocol = entity.Protocol;
            PhyNum = entity.PhyNum;
            Status = entity.Status;
            Notes = entity.Notes;
            TimeIn = entity.TimeIn;
            TimeOut = entity.TimeOut;
        }

        public int LogNum;
        public string Practice;
        public string Clinic;
        public int PatNum;
        private DateTime SvcDate;
        public string VisitType;
        public int CmpNum;
        public string Protocol;
        public int PhyNum;
        public string Status;
        public string Notes;
        public DateTime TimeIn;
        public DateTime? TimeOut;
    }
    public class HomeModule : NancyModule
    {
        protected static string MyNameSpace = "PV.Data.Standard.DataManagers";

        public HomeModule()
        {
            Get["/"] = _ => "Log Detail";
            Get["GetLogDetail/{logDetailPk}&{environment}"] = parameters =>
            {
                var results = GetLogDetail(parameters.logDetailPk, parameters.environment);
                return new JsonResponse(results, new DefaultJsonSerializer());

            };
            Get["GetProtocolInfoForLogDetailPk/{logDetailPk}&{environment}"] = parameters =>
            {
                var results = GetProtocolInfoForLogDetailPk(parameters.logDetailPk, parameters.environment);
                return new JsonResponse(results, new DefaultJsonSerializer());

            };
        }

        private LogDetailModel GetLogDetail(string logDetailPk, string environment)
        {
            Guid logDetailPkGuid;
            if (!Guid.TryParse(logDetailPk, out logDetailPkGuid))
            {
                return null;
            }

            using (IDataAccessAdapter adapter = AdapterFactory.CreateAdapter(MyNameSpace, environment))
            {

                    var toFetch = new LogDetailEntity(logDetailPkGuid);
                    var prefetch = new PrefetchPath2(EntityType.LogDetailEntity) { LogDetailEntity.PrefetchPathPhysician };
                    var fetchResult = adapter.FetchEntity(toFetch, prefetch);
                    return !fetchResult ? null : new LogDetailModel(toFetch);

            }
        }

        private object GetProtocolInfoForLogDetailPk(string logDetailPk, string environment)
        {
            Guid logDetailPkGuid;
            if (!Guid.TryParse(logDetailPk, out logDetailPkGuid))
            {
                return null;
            }

            var typedView = new LogDetailProtocolInfoTypedView();
            var filter = new RelationPredicateBucket();
            filter.PredicateExpression.Add(LogDetailProtocolInfoFields.LogDetailPk == logDetailPkGuid);


            using (IDataAccessAdapter adapter = AdapterFactory.CreateAdapter(MyNameSpace, environment))
            {
                adapter.FetchTypedView(typedView.GetFieldsInfo(), typedView, filter, 0, true);
            }

            return typedView;
        }

        
    }
}
